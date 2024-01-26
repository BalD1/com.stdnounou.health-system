using UnityEngine;

namespace StdNounou.Health
{
	[CreateAssetMenu(fileName = "New DamagesType", menuName = "StdNounou/Scriptables/Health/DamagesType", order = 0)]
	public class SO_DamagesType : ScriptableObject
	{
		[field: SerializeField] public string ID { get; private set; }
	} 
}