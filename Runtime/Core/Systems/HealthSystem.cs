using System;
using System.Collections.Generic;
using UnityEngine;
using StdNounou.Core;
using StdNounou.Stats;
using static StdNounou.Health.IDamageable;

namespace StdNounou.Health
{
    [RequireComponent(typeof(MonoStatsHandler))]
    public class HealthSystem : MonoBehaviourEventsHandler, IDamageable
    {
        [SerializeField] private GameObject owner;
        [field: SerializeField] public MonoStatsHandler Stats { get; private set; }
        [SerializeField] private SO_DamageCalculator damageCalculator;

        public float CurrentHealth { get; protected set; }
        public float CurrentMaxHealth { get; protected set; }

        public Dictionary<string, TickDamages> UniqueTickDamages { get; private set; } = new();
        public Dictionary<string, List<TickDamages>> StackableTickDamages { get; private set; } = new();

        [field: SerializeField] public Vector3 HealthPopupOffset { get; private set; }

        public float InvincibilityTimer { get; protected set; }

        public event Action<InflictedDamagesData> OnTookDamages;
        public event Action<float> OnHealed;
        public event Action OnDeath;
        public event Action OnRevive;

        private void Reset()
        {
            Stats = this.GetComponent<MonoStatsHandler>();
        }

        protected override void EventsSubscriber()
        {
            Stats.StatsHandler.OnStatChange += OnStatChange;
        }

        protected override void EventsUnSubscriber()
        {
            Stats.StatsHandler.OnStatChange -= OnStatChange;
        }

        protected override void Awake()
        {
            if (Stats == null) Stats = this.GetComponent<MonoStatsHandler>();
            base.Awake();
        }

        private void Start()
        {
            Setup();
        }

        protected virtual void Update()
        {
            if (InvincibilityTimer > 0) InvincibilityTimer -= Time.deltaTime;
        }

        protected virtual void OnStatChange(StatChangeEventArgs args)
        {
            switch (args.StatKey)
            {
                case E_StatsKeys.Health:
                    UpdateMaxHealth(args.FinalValue, true);
                    break;
            }
        }

        protected virtual void Setup()
        {
            if (!Stats.StatsHandler.TryGetFinalStat(E_StatsKeys.Health, out float maxHealth))
            {
                this.LogError("Health stat was not found in stats handler.");
                maxHealth = -1;
            }
            UpdateMaxHealth(maxHealth, false);
            CurrentHealth = maxHealth;
        }

        public void UpdateMaxHealth(float newHealth, bool healDifference)
        {
            float pastHealth = CurrentMaxHealth;

            Stats.StatsHandler.BaseStats.TryGetHigherAllowedValue(E_StatsKeys.Health, out float higherAllowedMaxHealth);
            CurrentMaxHealth = Mathf.Clamp(CurrentHealth + newHealth, 0, higherAllowedMaxHealth);
            if (healDifference)
            {
                float diffenrece = newHealth - pastHealth;
                if (diffenrece > 0)
                    this.Heal(diffenrece, false);
            }
        }

        public virtual bool TryInflictDamages(DamagesData damagesData)
        {
            if (!IsAlive()) return false;
            if (InvincibilityTimer > 0) return false;
            if (this.Stats.StatsHandler.GetAffiliation() != null && damagesData.DamagerAffiliation != null)
                if (!this.Stats.StatsHandler.GetAffiliation().AllowsInteractionsWith(damagesData.DamagerAffiliation)) return false;

            InflictDamages(damagesData);
            return true;
        }
        public virtual void InflictDamages(DamagesData damagesData)
        {
            if (!damageCalculator.TryCalculateDamages(this.Stats.StatsHandler, damagesData.Stats, damagesData.DamagerAffiliation, damagesData.AttackAttributes, out float damages, out bool isCrit))
                return;
            if (damagesData.Stats.TryGetValue(E_StatsKeys.KnockbackForce, out float force))
            {
                SO_Affiliation selfAffiliation = this.Stats.StatsHandler.GetAffiliation();
                selfAffiliation.TryGetModifiedStat(damagesData.DamagerAffiliation, E_StatsKeys.KnockbackForce, force, true);
                damagesData.DamagerAffiliation.TryGetModifiedStat(selfAffiliation, E_StatsKeys.KnockbackForce, force, false);
            }
            this.CurrentHealth -= damages;
            this.OnTookDamages?.Invoke(new InflictedDamagesData(
                damager: damagesData.Damager,
                damagerAffiliation: damagesData.DamagerAffiliation,
                attackAttributes: damagesData.AttackAttributes,
                damagesType: damagesData.DamagesType,
                damages: damages,
                isCrit: isCrit,
                damageDirection: damagesData.DamagesDirection,
                knockbackForce: force
                ));

            if (CurrentHealth <= 0) Kill();
        }

        public virtual bool TryInflictDamages(float damages, bool isCrit)
        {
            if (!IsAlive()) return false;
            if (InvincibilityTimer > 0) return false;
            InflictDamages(damages, isCrit);
            return true;
        }

        public virtual void InflictDamages(float damages, bool isCrit)
        {
            CurrentHealth -= damages;
            this.OnTookDamages?.Invoke(new(null, damages, isCrit));
            if (CurrentHealth <= 0) Kill();
        }

        public void Heal(float amount, bool isCrit)
        {
            if (!IsAlive()) return;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, CurrentMaxHealth);
            this.OnHealed?.Invoke(amount);
        }

        public bool IsAlive()
            => CurrentHealth > 0;

        public void Revive()
        {
            Stats.StatsHandler.TryGetFinalStat(E_StatsKeys.Health, out float maxHealth);
            Revive(maxHealth);
        }

        public void Revive(float hpToGive)
        {
            this.CurrentHealth = hpToGive;
            this.OnRevive?.Invoke();
        }

        public void Kill()
        {
            this.OnDeath?.Invoke();
        }

        public bool TryAddTickDammages(SO_TickDamagesData data, StatsHandler originStatsHandler)
        {
            if (data.Stackable)
            {
                if (!StackableTickDamages.ContainsKey(data.ID))
                    StackableTickDamages.Add(data.ID, new List<TickDamages>());

                StackableTickDamages[data.ID].Add(new TickDamages(data, this, owner, originStatsHandler));
                return true;
            }

            if (UniqueTickDamages.ContainsKey(data.ID)) return false;
            UniqueTickDamages.Add(data.ID, new TickDamages(data, this, owner, originStatsHandler));

            return true;
        }

        public void RemoveTickDamage(TickDamages tick)
        {
            if (tick.Data.Stackable)
            {
                StackableTickDamages[tick.Data.ID].Remove(tick);
                return;
            }

            UniqueTickDamages.Remove(tick.Data.ID);
        }

        public void SetInvincibilityTimer(float time)
            => InvincibilityTimer = time;

        public Vector2 GetHealthPopupPosition()
            => this.transform.position + (Vector3)HealthPopupOffset;

        public void AddOnDeathListener(Action listener)
        {
            OnDeath += listener;
        }

        public void RemoveOnDeathListener(Action listener)
        {
            OnDeath -= listener;
        }

        public Transform GetTransform()
            => this.transform;

        #region EDITOR

#if UNITY_EDITOR
        [SerializeField] protected bool ED_debugMode;
#endif

        protected virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!ED_debugMode) return;

            Vector3 healthBordersSize = new Vector3(0.75f, 0.5f);
            Gizmos.DrawWireCube(this.transform.position + HealthPopupOffset, healthBordersSize);

            Color c = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.red;

            Vector3 centeredPosition = this.transform.position + HealthPopupOffset;

            if (UnityEditor.SceneView.currentDrawingSceneView == null) return;

            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(centeredPosition);

            Vector3 textOffset = new Vector3(-36, 7.5f);
            Camera cam = UnityEditor.SceneView.currentDrawingSceneView.camera;
            if (cam)
                centeredPosition = cam.ScreenToWorldPoint(cam.WorldToScreenPoint(centeredPosition) + textOffset);

            UnityEditor.Handles.Label(centeredPosition, "Health Popup");

            UnityEditor.Handles.color = c;
#endif
        }

        #endregion
    } 
}
