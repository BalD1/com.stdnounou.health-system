using StdNounou.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StdNounou.Health
{
    public interface IDamageable
    {
        public abstract void UpdateMaxHealth(float newMaxHealth, bool healDifference);
        public abstract bool TryInflictDamages(DamagesData damagesData);
        public abstract void InflictDamages(DamagesData damagesData);
        public abstract bool TryInflictDamages(float amount, bool isCrit);
        public abstract void InflictDamages(float damages, bool isCrit);
        public abstract void Heal(float amount, bool isCrit);
        public abstract void Kill();
        public abstract void Revive();
        public abstract void Revive(float hpToGive);
        public bool IsAlive();
        public bool TryAddTickDammages(SO_TickDamagesData data, StatsHandler originStatsHandler);
        public void RemoveTickDamage(TickDamages tick);

        public void AddOnDeathListener(Action listener);
        public void RemoveOnDeathListener(Action listener);

        public Transform GetTransform();

        public class DamagesData : EventArgs
        {
            public object Damager { get; private set; }
            public SO_Affiliation DamagerAffiliation { get; private set; }
            public SO_DamagesType DamagesType { get; private set; }
            public Dictionary<E_StatsKeys, float> Stats { get; private set; }
            public Vector3 DamagesDirection { get; private set; }

            public DamagesData(object damager, StatsHandler statsHandler, SO_DamagesType damagesType, Vector3 damagesDirection)
            {
                this.Damager = damager;
                this.DamagerAffiliation = statsHandler.GetAffiliation();
                this.DamagesType = damagesType;
                this.Stats = new Dictionary<E_StatsKeys, float>();
                foreach (var item in statsHandler.BrutFinalStats.Keys)
                {
                    statsHandler.TryGetFinalStat(item, out float val);
                    Stats.Add(item, val);
                }
                DamagesDirection = damagesDirection;
            }
            public DamagesData(object damager, SO_Affiliation damagerAffiliation, SO_DamagesType damagesType, Dictionary<E_StatsKeys, float> stats, Vector3 damagesDirection)
            {
                Damager = damager;
                DamagerAffiliation = damagerAffiliation;
                DamagesType = damagesType;
                Stats = stats;
                DamagesDirection = damagesDirection;
            }
            public DamagesData(object damager, SO_Affiliation damagerAffiliation, SO_DamagesType damagesType, Dictionary<E_StatsKeys, float> stats)
            {
                Damager = damager;
                DamagerAffiliation = damagerAffiliation;
                DamagesType = damagesType;
                Stats = stats;
            }
        }

        public class InflictedDamagesData : EventArgs
        {
            public object Damager { get; private set; }
            public SO_Affiliation DamagerAffiliation { get; private set; }
            public SO_DamagesType DamagesType { get; private set; }
            public bool IsCrit { get; private set; }
            public float Damages { get; private set; }
            public Vector3 DamagesDirection { get; private set; }
            public float KnockbackForce { get; private set; }

            public InflictedDamagesData()
            {
                this.DamagerAffiliation = null;
                this.DamagesType = null;
                this.Damages = -1;
            }
            public InflictedDamagesData(object damager, float damages, bool isCrit)
            {
                this.Damager = damager;
                this.Damages = damages;
                this.IsCrit = isCrit;
            }
            public InflictedDamagesData(object damager, SO_Affiliation damagerAffiliation, SO_DamagesType damagesType, float damages, bool isCrit, Vector3 damageDirection, float knockbackForce)
            {
                this.Damager = damager;
                this.DamagerAffiliation = damagerAffiliation;
                this.DamagesType = damagesType;
                this.Damages = damages;
                this.IsCrit = isCrit;
                this.DamagesDirection = damageDirection;
                KnockbackForce = knockbackForce;
            }
        }
    } 
}
