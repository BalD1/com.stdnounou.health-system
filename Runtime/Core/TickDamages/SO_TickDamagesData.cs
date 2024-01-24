using UnityEngine;
using StdNounou.Core;
using StdNounou.Stats;

namespace StdNounou.Health
{
    [CreateAssetMenu(fileName = "New TickDamages Data", menuName = "StdNounou/Scriptables/TickDamages/Data")]
    public class SO_TickDamagesData : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }

        [field: SerializeField] public SO_DamagesType DamagesType { get; private set; }
        [field: SerializeField] public SO_Attribute[] Attributes { get; private set; }

        [field: SerializeField] public bool Stackable { get; private set; }
        [field: SerializeField, Range(0, 100)] public float ChancesToApply { get; private set; } = 50;

        [field: SerializeField] public int TicksLifetime { get; private set; }
        [field: SerializeField, Min(1)] public int RequiredTicksToTrigger { get; private set; }

        [field: SerializeField] public S_TickStat Damages { get; private set; }
        [field: SerializeField] public S_TickStat CritChances { get; private set; }
        [field: SerializeField] public S_TickStat CritMultiplier { get; private set; }


        public enum E_TickStatType
        {
            Fixed,
            Additive,
            Multiplier,
        }
        [System.Serializable]
        public struct S_TickStat
        {
            [field: SerializeField] public float Value { get; private set; }
            [field: SerializeField] public E_TickStatType StatType { get; private set; }
        }

        [field: SerializeField] public ParticlesPlayer Particles { get; private set; }

        public float CalculateFinalStatValue(S_TickStat tickStat, E_StatsKeys statKey, float externalStat)
        {
            float finalValue = -1;
            switch (tickStat.StatType)
            {
                case E_TickStatType.Fixed:
                    finalValue = tickStat.Value;
                    break;
                case E_TickStatType.Additive:
                    finalValue = tickStat.Value + externalStat;
                    break;
                case E_TickStatType.Multiplier:
                    finalValue = tickStat.Value * externalStat;
                    break;
            }
            return finalValue;
        }
    } 
}