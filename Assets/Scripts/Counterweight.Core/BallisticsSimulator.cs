using UnityEngine;

namespace Counterweight.Core
{
    /// <summary>
    /// Forward-simulates the projectile trajectory from a release point under
    /// gravity and linear drag. Pure function — no Unity engine state, no
    /// allocations (caller-provided buffer).
    ///
    /// The integration matches what Unity's Rigidbody would do step-by-step:
    ///   v += g * dt
    ///   v *= max(0, 1 - linearDamping * dt)
    ///   p += v * dt
    /// so the simulated path is a close visual match for the actual flight.
    /// </summary>
    public static class BallisticsSimulator
    {
        /// <summary>
        /// Fills <paramref name="buffer"/> with simulated path points starting
        /// from the release origin. Stops early when the path goes below
        /// <paramref name="groundY"/> or when the buffer is full.
        /// </summary>
        /// <returns>Number of points actually written to the buffer.</returns>
        public static int SimulatePath(
            TrebuchetConfig cfg,
            Vector3 origin,
            Vector3 launchForward,
            float powerMultiplier,
            float linearDamping,
            Vector3[] buffer,
            float dt = 0.05f,
            float groundY = float.NegativeInfinity)
        {
            if (cfg == null || buffer == null || buffer.Length == 0)
            {
                return 0;
            }
            if (dt <= 0f)
            {
                return 0;
            }

            Vector3 velocity = BallisticsSolver.ComputeReleaseVelocity(cfg, launchForward, powerMultiplier);
            Vector3 position = origin;
            Vector3 gravity = new Vector3(0f, -BallisticsSolver.Gravity, 0f);

            buffer[0] = position;
            int count = 1;

            while (count < buffer.Length)
            {
                velocity += gravity * dt;
                float dragFactor = Mathf.Max(0f, 1f - linearDamping * dt);
                velocity *= dragFactor;
                position += velocity * dt;
                buffer[count++] = position;
                if (position.y <= groundY)
                {
                    break;
                }
            }

            return count;
        }
    }
}
