using Counterweight.Core;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Spawns a configured projectile prefab at a release Transform and gives it
    /// the velocity computed by <see cref="BallisticsSolver"/>.
    /// </summary>
    public sealed class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private ProjectileConfig projectileConfig;

        public void Spawn(TrebuchetConfig trebuchetConfig, Transform release)
        {
            if (projectileConfig == null || projectileConfig.prefab == null)
            {
                Debug.LogError("[ProjectileSpawner] Missing ProjectileConfig or prefab.", this);
                return;
            }
            if (trebuchetConfig == null)
            {
                Debug.LogError("[ProjectileSpawner] Missing TrebuchetConfig.", this);
                return;
            }
            if (release == null)
            {
                Debug.LogError("[ProjectileSpawner] Missing release Transform.", this);
                return;
            }

            GameObject instance = Instantiate(projectileConfig.prefab, release.position, release.rotation);

            if (!instance.TryGetComponent(out Rigidbody rb))
            {
                Debug.LogError("[ProjectileSpawner] Spawned prefab has no Rigidbody.", instance);
                return;
            }

            rb.mass = projectileConfig.mass;
            rb.linearDamping = projectileConfig.linearDamping;
            rb.angularDamping = projectileConfig.angularDamping;
            rb.useGravity = true;

            Vector3 velocity = BallisticsSolver.ComputeReleaseVelocity(trebuchetConfig, release.forward);
            rb.linearVelocity = velocity;
        }
    }
}
