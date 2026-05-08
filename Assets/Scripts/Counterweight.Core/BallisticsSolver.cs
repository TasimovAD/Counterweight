using UnityEngine;

namespace Counterweight.Core
{
    /// <summary>
    /// Pure-function ballistics math. No Time, no Random, no MonoBehaviour.
    /// Easy to unit-test in EditMode.
    ///
    /// Projectile mass is taken as an explicit parameter so different ammo types
    /// (stone / clay pot / wooden ball) can use the same trebuchet config.
    /// </summary>
    public static class BallisticsSolver
    {
        public const float Gravity = 9.81f;

        /// <summary>
        /// Velocity vector to apply to the projectile at sling release.
        /// </summary>
        /// <param name="cfg">Trebuchet build (counterweight, arm, efficiency, release angle).</param>
        /// <param name="projectileMass">Mass of the loaded projectile (kg).</param>
        /// <param name="launchForward">Aim direction in world space; Y component is flattened.</param>
        /// <param name="powerMultiplier">Player-controlled power knob; linearly scales counterweight energy. 1.0 = nominal.</param>
        public static Vector3 ComputeReleaseVelocity(
            TrebuchetConfig cfg,
            float projectileMass,
            Vector3 launchForward,
            float powerMultiplier = 1f)
        {
            if (cfg == null) return Vector3.zero;

            float speed = ComputeReleaseSpeed(cfg, projectileMass, powerMultiplier);
            if (speed <= 0f) return Vector3.zero;

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
        /// Scalar release speed (m/s) under the energy approximation
        /// PE_counterweight × powerMultiplier × efficiency = KE_projectile.
        /// </summary>
        public static float ComputeReleaseSpeed(TrebuchetConfig cfg, float projectileMass, float powerMultiplier = 1f)
        {
            if (cfg == null || projectileMass <= 0f || cfg.launchEfficiency <= 0f || powerMultiplier <= 0f)
            {
                return 0f;
            }

            float potentialEnergy = cfg.counterweightMass * powerMultiplier * Gravity * cfg.armLength;
            float kineticEnergy = potentialEnergy * cfg.launchEfficiency;
            float speedSquared = 2f * kineticEnergy / projectileMass;
            return speedSquared > 0f ? Mathf.Sqrt(speedSquared) : 0f;
        }
    }
}
