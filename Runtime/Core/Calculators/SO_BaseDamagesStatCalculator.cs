using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health
{
    [CreateAssetMenu(fileName = "New Damages Stat Calculator", menuName = "StdNounou/Scriptables/Health/StatCalculator/BaseDamages")]
    public class SO_BaseDamagesStatCalculator : SO_SingleStatCalculator
    {
        public override void CalculateStat(SO_DamageCalculator calculator, ref float currentDamagesValue)
        {
            if (!TryGetStatFromDamager(calculator, E_StatsKeys.Damages, out float result)) return;
            CalculateStat(calculator, E_StatsKeys.Damages, ref result);
            currentDamagesValue += result;
        }
    }
}