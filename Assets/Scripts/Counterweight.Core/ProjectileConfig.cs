using UnityEngine;

namespace Counterweight.Core
{
    /// <summary>
    /// Description of a projectile prefab and its physics properties.
    /// </summary>
    [CreateAssetMenu(menuName = "Counterweight/Projectile Config", fileName = "ProjectileConfig")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        [Tooltip("Prefab spawned at the sling release point. Must contain a Rigidbody.")]
        public GameObject prefab;

        [Min(0.01f)] public float mass = 8f;
        [Min(0f)] public float linearDamping = 0.05f;
        [Min(0f)] public float angularDamping = 0.05f;
    }
}
