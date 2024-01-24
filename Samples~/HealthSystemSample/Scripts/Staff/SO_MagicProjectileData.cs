using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health.Samples
{
	[CreateAssetMenu(fileName = "New MagicProjectile Data", menuName = "StdNounou/Scriptables/Projectiles/Magic Proj Data")]
	public class SO_MagicProjectileData : ScriptableObject
	{
		[field: SerializeField] public string ID {  get; private set; }
		[field: SerializeField] public SO_Attribute[] Attributes { get; private set; }
		[field: SerializeField] public Color color;
		[field: SerializeField, Tooltip("How much of the sender damages stat should the projectile keep ?"), Range(0,100)] public float OwnerDamagesPercentage { get; private set; }

		[field: SerializeField] public SO_TickDamagesData TickDamagesData { get; private set; }
		[field: SerializeField] public float TicksLifetime { get; private set; } = 20;
	} 
}