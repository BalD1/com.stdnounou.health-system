using StdNounou.Core.ComponentsHolder;
using StdNounou.Core;
using StdNounou.Health.Core;
using UnityEngine;

namespace StdNounou.Health
{
    public class HealthSystemKnockback : HealthSystemKnockback_Core
    {
        private Rigidbody body;

        protected override void SetBody()
        {
            E_HolderResult result = componentHolder.HolderTryGetComponent(E_ComponentsKeys.Rigidbody, out body);
            if (result != E_HolderResult.Success)
                this.LogError($"Could not find health system component in holder {componentsHolderObj} : {result}");
        }

        protected override void ApplyKnockback(Vector3 direction, float force)
        {
            body.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}