using UnityEngine;
using StdNounou.Stats;
using System.Collections.Generic;
using StdNounou.Core;
using StdNounou.Tick;

namespace StdNounou.Health.Samples
{
    public class MagicProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float force;

        private SO_MagicProjectileData data;

        private GameObject sender;
        private StatsHandler senderStats;

        private Timer lifeTimer;

        public void Launch(SO_MagicProjectileData data, GameObject sender, StatsHandler senderStats)
        {
            this.data = data;
            this.spriteRenderer.color = data.color;
            this.rb.AddForce(Vector2.right * force, ForceMode2D.Impulse);

            this.sender = sender;
            this.senderStats = senderStats;

            lifeTimer = new Timer(data.TicksLifetime, () => Destroy(this.gameObject));
            lifeTimer.Start();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) ApplyDamages(damageable);
        }

        private void ApplyDamages(IDamageable target)
        {
            Dictionary<E_StatsKeys, float> projectileStats = new Dictionary<E_StatsKeys, float>();

            senderStats.TryGetFinalStat(E_StatsKeys.Damages, out float senderDamages);
            senderStats.TryGetFinalStat(E_StatsKeys.CritChances, out float senderCritChances);
            senderStats.TryGetFinalStat(E_StatsKeys.CritMultiplier, out float senderCritMultiplier);

            senderDamages = data.OwnerDamagesPercentage / 100 * senderDamages;
            projectileStats.Add(E_StatsKeys.Damages, senderDamages);
            projectileStats.Add(E_StatsKeys.CritChances, senderCritChances);
            projectileStats.Add(E_StatsKeys.CritMultiplier, senderCritMultiplier);

            IDamageable.DamagesData damageData = new IDamageable.DamagesData(
                damager: sender,
                damagerAffiliation: senderStats.GetAffiliation(),
                attackAttributes: data.Attributes,
                damagesType: null,
                stats: projectileStats
                );

            target.TryInflictDamages(damageData);
            TryApplyTicks(target);

            lifeTimer.Stop(callEndAction: true);
            lifeTimer = null;
        }

        private void TryApplyTicks(IDamageable target)
        {
            SO_TickDamagesData tickData = data.TickDamagesData;
            if (tickData == null) return;
            if (tickData.ChancesToApply <= 0) return;
            if (!RandomExtensions.PercentageChance(tickData.ChancesToApply)) return;

            target.TryAddTickDammages(tickData, senderStats);
        }
    } 
}