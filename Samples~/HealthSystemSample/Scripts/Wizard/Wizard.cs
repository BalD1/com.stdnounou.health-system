using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using StdNounou.Core.ComponentsHolder;
using System;
using UnityEngine;

namespace StdNounou.Health.Samples
{
    [SelectionBase]
    public class Wizard : MonoBehaviour, IComponentHolder
    {
        [field: SerializeField] public SerializedDictionary<E_ComponentsKeys, Component> Components { get; private set; }
        public event Action<ComponentChangeEventArgs> OnComponentModified;

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