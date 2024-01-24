using StdNounou.Core;
using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health
{
    public abstract class SO_SingleStatCalculator : ScriptableObject
    {
        [SerializeField] protected bool calculateReceivingAffiliationStatModifierFirst;

        public abstract void CalculateStat(SO_DamageCalculator calculator, ref float currentDamagesValue);

        protected bool TryGetStatFromDamager(SO_DamageCalculator calculator, E_StatsKeys statKey, out float result)
        {
            if (!calculator.GetDamagerStats.TryGetValue(statKey, out result))
            {
                this.LogError($"Stats Handler {calculator.GetDamagerStats} doesn't have stat {statKey}.");
                return false;
            }
            return true;
        }
        protected bool TryGetStatFromOwner(SO_DamageCalculator calculator, E_StatsKeys statKey, out float result)
        {
            if (!calculator.GetOwnerHandler.BrutFinalStats.TryGetValue(statKey, out result))
            {
                this.LogError($"Stats Handler {calculator.GetOwnerHandler} doesn't have stat {statKey}.");
                return false;
            }
            return true;
        }

        protected void GetModifierStatFromAffliation(SO_DamageCalculator calculator, E_StatsKeys statKey, ref float stat)
        {
            SO_Affiliation ownerAffiliation = calculator.GetOwnerHandler.GetAffiliation();
            if (calculateReceivingAffiliationStatModifierFirst)
            {
                stat = ownerAffiliation.TryGetModifiedStat(calculator.GetDamagerAffiliation, statKey, stat, true);
                stat = calculator.GetDamagerAffiliation.TryGetModifiedStat(ownerAffiliation, statKey, stat, false);
            }
            else
            {
                stat = calculator.GetDamagerAffiliation.TryGetModifiedStat(ownerAffiliation, statKey, stat, false);
                stat = ownerAffiliation.TryGetModifiedStat(calculator.GetDamagerAffiliation, statKey, stat, true);
            }
        }
    }
}