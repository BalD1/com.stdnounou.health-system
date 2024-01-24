using StdNounou.Core;
using StdNounou.Core.ComponentsHolder;
using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health.Core
{
    [RequireComponent(typeof(HealthSystem))]
    public abstract class HealthSystemKnockback_Core : MonoBehaviourEventsHandler
    {
        [SerializeField] protected GameObject componentsHolderObj;
        protected IComponentHolder componentHolder;
        protected HealthSystem healthSystem;

        protected override void EventsSubscriber()
        {
            healthSystem.OnTookDamages += PerformKnockback;
        }

        protected override void EventsUnSubscriber()
        {
            healthSystem.OnTookDamages -= PerformKnockback;
        }

        protected override void Awake()
        {
            componentHolder = componentsHolderObj.GetComponent<IComponentHolder>();
            SetHealthSystem();
            SetBody();
            base.Awake();
        }

        protected virtual void SetHealthSystem()
        {
            E_HolderResult result = componentHolder.HolderTryGetComponent(E_ComponentsKeys.HealthSystem, out healthSystem);
            if (result != E_HolderResult.Success)
                this.LogError($"Could not find health system component in holder {componentsHolderObj} : {result}");
        }
        protected abstract void SetBody();

        protected void PerformKnockback(IDamageable.InflictedDamagesData damagesData)
        {
            float finalForce = CalculateKnockbackForce(damagesData);
            if (finalForce <= 0) return;
            ApplyKnockback(damagesData.DamagesDirection, finalForce);
        }

        protected abstract void ApplyKnockback(Vector3 direction, float force);

        protected virtual float CalculateKnockbackForce(IDamageable.InflictedDamagesData damagesData)
        {
            StatsHandler statsHandler = healthSystem.Stats.StatsHandler;
            if (!statsHandler.TryGetFinalStat(E_StatsKeys.Weight, out float weight)) weight = 0;
            statsHandler.GetAffiliation().TryGetModifiedStat(damagesData.DamagerAffiliation, E_StatsKeys.Weight, weight, true);
            damagesData.DamagerAffiliation.TryGetModifiedStat(statsHandler.GetAffiliation(), E_StatsKeys.Weight, weight, false);

            if (!statsHandler.TryGetFinalStat(E_StatsKeys.KnockbackForce, out float force)) force = 0;
            statsHandler.GetAffiliation().TryGetModifiedStat(damagesData.DamagerAffiliation, E_StatsKeys.KnockbackForce, force, true);
            damagesData.DamagerAffiliation.TryGetModifiedStat(statsHandler.GetAffiliation(), E_StatsKeys.KnockbackForce, force, false);

            return force - weight;
        }
    }
}
