using StdNounou.Core;
using StdNounou.Stats;
using StdNounou.Tick;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StdNounou.Health
{
    [System.Serializable]
    public class TickDamages : ITickable, IDisposable
    {
        public SO_TickDamagesData Data { get; private set; }
        private object damager;
        private StatsHandler externalStatsHandler;
        private IDamageable targetDamageable;

        private int currentTicks;

        private ParticlesPlayer particles;

        public TickDamages(SO_TickDamagesData data, IDamageable targetDamageable, object damager, StatsHandler externalStatsHandler)
        {
            this.Data = data;
            this.damager = damager;
            this.externalStatsHandler = externalStatsHandler;
            this.targetDamageable = targetDamageable;
            this.targetDamageable.AddOnDeathListener(OnEnd);

            TickManagerEvents.OnTick += OnTick;
            particles = Data.Particles?.Create(targetDamageable.GetTransform());
        }

        public void Dispose()
        {
            TickManagerEvents.OnTick -= OnTick;
        }

        public virtual void OnTick(int tick)
        {
            if (currentTicks % Data.RequiredTicksToTrigger == 0)
            {
                ApplyDamages();
            }

            if (currentTicks >= Data.TicksLifetime) OnEnd();
            currentTicks++;
        }

        protected virtual void ApplyDamages()
        {
            IDamageable.DamagesData damagesData = new IDamageable.DamagesData(damager, externalStatsHandler.GetAffiliation(), Data.Attributes, Data.DamagesType, CreateStatsDict());
            targetDamageable.TryInflictDamages(damagesData);
        }

        private Dictionary<E_StatsKeys, float> CreateStatsDict()
        {
            Dictionary<E_StatsKeys, float> damagesStats = new Dictionary<E_StatsKeys, float>();
            foreach (var item in externalStatsHandler.BrutFinalStats.Keys)
            {
                externalStatsHandler.TryGetFinalStat(item, out float val);
                damagesStats.Add(item, val);
            }

            ModifyStat(Data.Damages, E_StatsKeys.Damages);
            ModifyStat(Data.CritChances, E_StatsKeys.CritChances);
            ModifyStat(Data.CritMultiplier, E_StatsKeys.CritMultiplier);

            return damagesStats;

            void ModifyStat(SO_TickDamagesData.S_TickStat stat, E_StatsKeys statKey)
            {
                if (!damagesStats.ContainsKey(statKey))
                {
                    this.LogError($"Stats handler {externalStatsHandler} doesn't have a stat {statKey}.");
                    return;
                }
                switch (stat.StatType)
                {
                    case SO_TickDamagesData.E_TickStatType.Fixed:
                        damagesStats[statKey] = stat.Value;
                        break;

                    case SO_TickDamagesData.E_TickStatType.Additive:
                        damagesStats[statKey] += stat.Value;
                        break;

                    case SO_TickDamagesData.E_TickStatType.Multiplier:
                        damagesStats[statKey] *= stat.Value;
                        break;
                }
            }
        }

        public void KillTick()
            => OnEnd();

        private void OnEnd()
        {
            TickManagerEvents.OnTick -= OnTick;
            if (targetDamageable != null)
            {
                targetDamageable.RemoveOnDeathListener(OnEnd);
                targetDamageable.RemoveTickDamage(this);
            }
            if (particles != null) GameObject.Destroy(particles.gameObject);
        }

        public float RemainingTimeInSeconds()
            => RemainingTicks() * TickManager.TICK_TIMER_MAX;

        public int RemainingTicks()
           => Data.TicksLifetime - currentTicks;
    } 
}