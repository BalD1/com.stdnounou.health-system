using StdNounou.Core;
using StdNounou.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StdNounou.Health
{
    [CreateAssetMenu(fileName = "New Damages Calculator", menuName = "StdNounou/Scriptables/Health/Damages Calculator")]
    public class SO_DamageCalculator : ScriptableObject
    {
        [NonSerialized] private StatsHandler ownerHandler;
        [NonSerialized] private Dictionary<E_StatsKeys, float> damagerStats;
        private SO_Affiliation damagerAffiliation;
        [NonSerialized] private SO_Affiliation.S_AffiliationModifiers affiliationModifiers;
        [NonSerialized] private bool hasAffiliationModifiers;
        public StatsHandler GetOwnerHandler { get => ownerHandler; }
        public Dictionary<E_StatsKeys, float> GetDamagerStats { get => damagerStats; }
        public SO_Affiliation GetDamagerAffiliation { get => damagerAffiliation; }
        public SO_Affiliation.S_AffiliationModifiers GetAffiliationModifiers { get => affiliationModifiers; }
        public bool GetHasAffiliationModifiers { get => hasAffiliationModifiers; }

        [SerializeField] private SO_SingleStatCalculator baseDamageCalculator;
        [SerializeField] private SO_SingleStatCalculator[] calculators;

        private bool isCrit_keep;

        public bool TryCalculateDamages(StatsHandler ownerHandler, Dictionary<E_StatsKeys, float> damagerStats, SO_Affiliation damagerAffiliation, out float damages, out bool isCrit)
        {
            if (ownerHandler.TryGetAffiliationModifiersOf(damagerAffiliation, out affiliationModifiers))
            {
                if (!affiliationModifiers.AllowInteractions)
                {
                    damages = 0;
                    isCrit = false;
                    ResetSO();
                    return false;
                }
                hasAffiliationModifiers = true;
            }
            this.ownerHandler = ownerHandler;
            this.damagerAffiliation = damagerAffiliation;
            this.damagerStats = damagerStats;
            damages = 0;
            isCrit = false;

            if (!damagerStats.ContainsKey(E_StatsKeys.Damages))
            {
                this.LogError($"Handler {ownerHandler} did not have a Damages Stat.");
                ResetSO();
                return false;
            }

            baseDamageCalculator.CalculateStat(this, ref damages);

            foreach (var item in calculators)
            {
                item.CalculateStat(this, ref damages);
            }

            isCrit = isCrit_keep;
            ResetSO();
            return true;
        }

        private void ResetSO()
        {
            isCrit_keep = false;
            hasAffiliationModifiers = false;
            this.affiliationModifiers = default;
            this.ownerHandler = null;
            this.damagerAffiliation = null;
            this.damagerStats = null;
        }

        public void SetIsCrit(bool isCrit)
            => this.isCrit_keep = isCrit;
    } 
}