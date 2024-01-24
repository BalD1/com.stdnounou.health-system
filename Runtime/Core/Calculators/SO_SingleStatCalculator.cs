using StdNounou.Core;
using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health
{
    public abstract class SO_SingleStatCalculator : ScriptableObject
    {
        [SerializeField] protected E_Calculation[] calculationOrder = new E_Calculation[]
            {
                E_Calculation.ReceivingAffiliationModfier,
                E_Calculation.InflictingAffiliationModifier,
                E_Calculation.AttributesModifiers
            };

        public enum E_Calculation
        {
            ReceivingAffiliationModfier,
            InflictingAffiliationModifier,
            AttributesModifiers,
        }

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

        protected void CalculateStat(SO_DamageCalculator calcualtor, E_StatsKeys statKey, ref float stat)
        {
            foreach (var item in calculationOrder) 
            {
                switch (item)
                {
                    case E_Calculation.ReceivingAffiliationModfier:
                        if (calcualtor.GetHasAffiliationModifiers)
                            GetModifierStatFromAffliation(calcualtor, statKey, true, ref stat);
                        break;
                    case E_Calculation.InflictingAffiliationModifier:
                        if (calcualtor.GetHasAffiliationModifiers)
                            GetModifierStatFromAffliation(calcualtor, statKey, false, ref stat);
                        break;
                    case E_Calculation.AttributesModifiers:
                        GetModifiedStatFromAttributes(calcualtor, statKey, ref stat);
                        break;
                }
            }
        }

        protected void GetModifierStatFromAffliation(SO_DamageCalculator calculator, E_StatsKeys statKey, bool receiver, ref float stat)
        {
            SO_Affiliation ownerAffiliation = calculator.GetOwnerHandler.GetAffiliation();
            if (receiver)
                stat = ownerAffiliation.TryGetModifiedStat(calculator.GetDamagerAffiliation, statKey, stat, true);
            else
                stat = calculator.GetDamagerAffiliation.TryGetModifiedStat(ownerAffiliation, statKey, stat, false);
        }

        protected void GetModifiedStatFromAttributes(SO_DamageCalculator calculator, E_StatsKeys statsKeys,  ref float stat)
        {
            foreach (var item in calculator.GetAttackAttributes)
            {
                stat = calculator.GetOwnerHandler.TryGetModifiedStatFromAttribute(item, statsKeys, stat);
            }
            
        }
    }
}