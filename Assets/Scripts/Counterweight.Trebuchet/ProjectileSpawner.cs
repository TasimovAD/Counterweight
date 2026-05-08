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
        [Tooltip("Used as a fallback when the controller doesn't pass a specific config (e.g. nothing loaded).")]
        [SerializeField] private ProjectileConfig defaultProjectileConfig;

        public ProjectileConfig DefaultProjectileConfig => defaultProjectileConfig;

        public void Spawn(TrebuchetConfig trebuchetConfig, ProjectileConfig projectileConfig, Transform release, float powerMultiplier = 1f)
        {
            ProjectileConfig effective = projectileConfig != null ? projectileConfig : defaultProjectileConfig;
            if (effective == null || effective.prefab == null)
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

            GameObject instance = Instantiate(effective.prefab, release.position, release.rotation);

            if (!instance.TryGetComponent(out Rigidbody rb))
            {
                Debug.LogError("[ProjectileSpawner] Spawned prefab has no Rigidbody.", instance);
                return;
            }

            rb.mass = effective.mass;
            rb.linearDamping = effective.linearDamping;
            rb.angularDamping = effective.angularDamping;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            Vector3 velocity = BallisticsSolver.ComputeReleaseVelocity(trebuchetConfig, effective.mass, release.forward, powerMultiplier);
            rb.linearVelocity = velocity;
        }
    }
}
