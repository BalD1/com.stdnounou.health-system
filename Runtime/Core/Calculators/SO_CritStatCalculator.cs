using StdNounou.Core;
using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health
{
    [CreateAssetMenu(fileName = "New Crit Stat Calculator", menuName = "StdNounou/Scriptables/Health/StatCalculator/Crit", order = 0)]
    public class SO_CritStatCalculator : SO_SingleStatCalculator
    {
        public override void CalculateStat(SO_DamageCalculator calculator, ref float currentDamagesValue)
        {
            if (!TryGetStatFromDamager(calculator, E_StatsKeys.CritChances, out float critChances)) return;
            if (!TryGetStatFromDamager(calculator, E_StatsKeys.CritMultiplier, out float critMultiplier)) return;
            CalculateStat(calculator, E_StatsKeys.CritChances, ref critChances);

            if (critChances <= 0) return;
            CalculateStat(calculator, E_StatsKeys.CritMultiplier, ref critMultiplier);
            if (!RandomExtensions.PercentageChance(critChances)) return;
            currentDamagesValue *= critMultiplier;
            calculator.SetIsCrit(true);
        }
    }
}