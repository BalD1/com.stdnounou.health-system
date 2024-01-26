using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health
{
	[CreateAssetMenu(fileName = "New DamagesReduction Stat Calculator", menuName = "StdNounou/Scriptables/Health/StatCalculator/DamagesReduction", order = 0)]
	public class SO_DamageReductionCalculator : SO_SingleStatCalculator
	{
        public override void CalculateStat(SO_DamageCalculator calculator, ref float currentDamagesValue)
        {
            if (!TryGetStatFromOwner(calculator, E_StatsKeys.DamageReduction, out float result)) return;
            CalculateStat(calculator, E_StatsKeys.DamageReduction, ref result);
            currentDamagesValue -= result;
        }
    } 
}