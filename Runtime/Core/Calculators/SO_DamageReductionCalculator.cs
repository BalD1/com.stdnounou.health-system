using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health
{
	[CreateAssetMenu(fileName = "New DamagesReduction Stat Calculator", menuName = "StdNounou/Scriptables/Health/StatCalculator/DamagesReduction")]
	public class SO_DamageReductionCalculator : SO_SingleStatCalculator
	{
        public override void CalculateStat(SO_DamageCalculator calculator, ref float currentDamagesValue)
        {
            if (!TryGetStatFromDamager(calculator, E_StatsKeys.DamageReduction, out float result)) return;
            if (!calculator.GetHasAffiliationModifiers) return;
            GetModifierStatFromAffliation(calculator, E_StatsKeys.DamageReduction, ref result);
            currentDamagesValue -= result;
        }
    } 
}