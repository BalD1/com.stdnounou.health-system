using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using StdNounou.Core.ComponentsHolder;
using System;
using UnityEngine;

namespace StdNounou.Health.Samples
{
    [SelectionBase]
    public class Dummy : MonoBehaviour, IComponentHolder
    {
        [field: SerializeField] public SerializedDictionary<E_ComponentsKeys, Component> Components { get; private set; } 
        public event Action<ComponentChangeEventArgs> OnComponentModified;

        private void Awake()
        {
            E_HolderResult res = HolderTryGetComponent(E_ComponentsKeys.HealthSystem, out HealthSystem healthSystem);
            if (res != E_HolderResult.Success) return;
            healthSystem.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            E_HolderResult healthSysRes = HolderTryGetComponent(E_ComponentsKeys.HealthSystem, out HealthSystem healthSystem);
            if (healthSysRes == E_HolderResult.Success) 
                healthSystem.Revive();
        }

        public void HolderChangeComponent<ExpectedType>(E_ComponentsKeys componentType, ExpectedType component) where ExpectedType : Component
        {
            this.HolderChangeComponent(Components, componentType, component, OnComponentModified);
        }

        public ExpectedType HolderGetComponent<ExpectedType>(E_ComponentsKeys component) where ExpectedType : Component
        {
            return this.HolderGetComponent<ExpectedType>(Components, component);
        }

        public E_HolderResult HolderTryGetComponent<ExpectedType>(E_ComponentsKeys component, out ExpectedType result) where ExpectedType : Component
        {
            return this.HolderTryGetComponent(Components, component, out result);
        }
    }
}