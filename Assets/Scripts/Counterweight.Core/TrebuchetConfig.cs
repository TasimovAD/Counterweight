using UnityEngine;

namespace Counterweight.Core
{
    /// <summary>
    /// Designer-tunable parameters for a trebuchet build.
    /// Real trebuchet motion is a coupled ODE; for cozy gameplay we approximate
    /// energy transfer as PE_counterweight * efficiency = KE_projectile.
    /// </summary>
    [CreateAssetMenu(menuName = "Counterweight/Trebuchet Config", fileName = "TrebuchetConfig")]
    public sealed class TrebuchetConfig : ScriptableObject
    {
        [Tooltip("Mass of the counterweight in kilograms.")]
        [Min(0f)] public float counterweightMass = 200f;

        [Tooltip("Effective drop height of the counterweight, approximated by the long-arm length in meters.")]
        [Min(0.01f)] public float armLength = 4f;

        [Tooltip("Sling length in meters. Reserved for later iterations (release timing).")]
        [Min(0f)] public float slingLength = 3f;

        [Tooltip("Angle above horizontal at which the projectile leaves the sling.")]
        [Range(0f, 89f)] public float releaseAngleDeg = 45f;

        [Tooltip("Fraction of counterweight potential energy converted to projectile kinetic energy.")]
        [Range(0f, 1f)] public float launchEfficiency = 0.55f;

        [Tooltip("Projectile mass in kilograms used for the energy equation.")]
        [Min(0.01f)] public float projectileMass = 8f;
    }
}
