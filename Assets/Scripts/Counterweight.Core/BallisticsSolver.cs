using UnityEngine;

namespace Counterweight.Core
{
    /// <summary>
    /// Pure-function ballistics math. No Time, no Random, no MonoBehaviour.
    /// This makes it trivially unit-testable in EditMode.
    /// </summary>
    public static class BallisticsSolver
    {
        public const float Gravity = 9.81f;

        /// <summary>
        /// Computes the velocity vector that should be applied to the projectile
        /// at the moment the sling releases.
        ///
        /// The launch direction is the supplied <paramref name="launchForward"/>
        /// rotated upward by <see cref="TrebuchetConfig.releaseAngleDeg"/>
        /// around the world-up-orthogonal right axis derived from forward.
        /// </summary>
        /// <param name="cfg">Trebuchet build parameters.</param>
        /// <param name="launchForward">
        /// Horizontal aim direction in world space. Y component is ignored
        /// for direction calculation; magnitude does not matter.
        /// </param>
        public static Vector3 ComputeReleaseVelocity(TrebuchetConfig cfg, Vector3 launchForward)
        {
            if (cfg == null)
            {
                return Vector3.zero;
            }

            float speed = ComputeReleaseSpeed(cfg);
            if (speed <= 0f)
            {
                return Vector3.zero;
            }

            Vector3 horizontal = new Vector3(launchForward.x, 0f, launchForward.z);
            if (horizontal.sqrMagnitude < 1e-6f)
            {
                horizontal = Vector3.forward;
            }
            horizontal.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, horizontal);
            Quaternion pitch = Quaternion.AngleAxis(-cfg.releaseAngleDeg, right);
            Vector3 direction = (pitch * horizontal).normalized;

            return direction * speed;
        }

        /// <summary>
        /// Scalar speed (m/s) of the projectile at release based on energy transfer.
        /// </summary>
        public static float ComputeReleaseSpeed(TrebuchetConfig cfg)
        {
            if (cfg == null || cfg.projectileMass <= 0f || cfg.launchEfficiency <= 0f)
            {
                return 0f;
            }

            float potentialEnergy = cfg.counterweightMass * Gravity * cfg.armLength;
            float kineticEnergy = potentialEnergy * cfg.launchEfficiency;
            float speedSquared = 2f * kineticEnergy / cfg.projectileMass;
            return speedSquared > 0f ? Mathf.Sqrt(speedSquared) : 0f;
        }
    }
}
