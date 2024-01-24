using StdNounou.Core;
using StdNounou.Core.ComponentsHolder;
using StdNounou.Core.Editor;
using StdNounou.Stats;
using UnityEngine;

namespace StdNounou.Health.Samples
{
    [SelectionBase]
    public class Staff : MonoBehaviour
    {
        [SerializeField] private GameObject ownerObj;
        private IComponentHolder owner;
        private StatsHandler ownerStatsHandler;

        [SerializeField] private Transform firePoint;

        [SerializeField] private MagicProjectile magicProjectile_PF;
        [SerializeField] private SO_MagicProjectileData baseMagicProjectileData;
        [SerializeField, ReadOnly] private SO_MagicProjectileData currentMagicProjectileData;

        private void Awake()
        {
            owner = ownerObj.GetComponent<IComponentHolder>();
            if (owner.HolderTryGetComponent(E_ComponentsKeys.StatsHandler, out MonoStatsHandler ownerMSH) == E_HolderResult.Success)
            {
                ownerStatsHandler = ownerMSH.StatsHandler;
            }
            currentMagicProjectileData = baseMagicProjectileData;
        }

        public void SetProjectileData(SO_MagicProjectileData data)
        {
            this.currentMagicProjectileData = data;
        }

        public void LaunchProjectile()
        {
            MagicProjectile proj = magicProjectile_PF?.Create(firePoint.transform.position);
            proj.Launch(currentMagicProjectileData, this.gameObject, ownerStatsHandler);
        }
    } 
}