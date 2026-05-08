using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Tunables for a single block — both HP-based damage and physics-based joint break thresholds.
    /// One ScriptableObject is typically shared by all blocks in a tower (per material).
    /// </summary>
    [CreateAssetMenu(menuName = "Counterweight/Block Config", fileName = "BlockConfig")]
    public sealed class BlockConfig : ScriptableObject
    {
        [Header("Mass / HP")]
        [Tooltip("Per-block Rigidbody mass. Game-balanced — real stone would be much heavier, but joint stability requires lighter blocks to keep the tower from collapsing under its own weight.")]
        [Min(0.1f)] public float mass = 5f;

        [Tooltip("Starting HP. Hits drain HP; HP <= 0 destroys all attached joints.")]
        [Min(0f)] public float maxHp = 20f;

        [Header("Impact damage")]
        [Tooltip("Collisions with impulse below this contribute no damage.")]
        [Min(0f)] public float damageThreshold = 20f;

        [Tooltip("Damage per unit of impulse above the threshold.")]
        [Min(0f)] public float damageMultiplier = 0.3f;

        [Header("Joint break (physics)")]
        [Tooltip("Force above which a joint connecting this block to a neighbor breaks instantly. Must comfortably exceed gravity load (N_layers * mass * 9.81) at the bottom of the tower.")]
        [Min(0f)] public float jointBreakForce = 1200f;

        [Tooltip("Torque above which a joint breaks instantly.")]
        [Min(0f)] public float jointBreakTorque = 1200f;
    }
}
