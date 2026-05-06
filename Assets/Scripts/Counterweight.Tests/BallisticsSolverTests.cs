using Counterweight.Core;
using NUnit.Framework;
using UnityEngine;

namespace Counterweight.Tests
{
    public sealed class BallisticsSolverTests
    {
        private static TrebuchetConfig MakeConfig(
            float counterweightMass = 200f,
            float armLength = 4f,
            float releaseAngleDeg = 45f,
            float launchEfficiency = 0.55f,
            float projectileMass = 8f)
        {
            var cfg = ScriptableObject.CreateInstance<TrebuchetConfig>();
            cfg.counterweightMass = counterweightMass;
            cfg.armLength = armLength;
            cfg.releaseAngleDeg = releaseAngleDeg;
            cfg.launchEfficiency = launchEfficiency;
            cfg.projectileMass = projectileMass;
            return cfg;
        }

        [Test]
        public void HeavierCounterweightIncreasesSpeed()
        {
            var light = MakeConfig(counterweightMass: 100f);
            var heavy = MakeConfig(counterweightMass: 400f);

            float vLight = BallisticsSolver.ComputeReleaseSpeed(light);
            float vHeavy = BallisticsSolver.ComputeReleaseSpeed(heavy);

            Assert.Greater(vHeavy, vLight);
        }

        [Test]
        public void HeavierProjectileDecreasesSpeed()
        {
            var light = MakeConfig(projectileMass: 4f);
            var heavy = MakeConfig(projectileMass: 16f);

            float vLight = BallisticsSolver.ComputeReleaseSpeed(light);
            float vHeavy = BallisticsSolver.ComputeReleaseSpeed(heavy);

            Assert.Greater(vLight, vHeavy);
        }

        [Test]
        public void ZeroEfficiencyProducesZeroSpeed()
        {
            var cfg = MakeConfig(launchEfficiency: 0f);
            float v = BallisticsSolver.ComputeReleaseSpeed(cfg);
            Assert.AreEqual(0f, v, 1e-6f);
        }

        [Test]
        public void ReleaseVelocityHasPositiveYWhenAngleIsPositive()
        {
            var cfg = MakeConfig(releaseAngleDeg: 45f);
            Vector3 v = BallisticsSolver.ComputeReleaseVelocity(cfg, Vector3.forward);
            Assert.Greater(v.y, 0f);
        }

        [Test]
        public void ReleaseSpeedMatchesEnergyEquation()
        {
            // PE = m * g * h = 200 * 9.81 * 4 = 7848 J
            // KE = 0.55 * 7848 = 4316.4 J
            // v = sqrt(2 * 4316.4 / 8) = sqrt(1079.1) ~ 32.85 m/s
            var cfg = MakeConfig();
            float expected = Mathf.Sqrt(2f * (200f * 9.81f * 4f) * 0.55f / 8f);
            float actual = BallisticsSolver.ComputeReleaseSpeed(cfg);
            Assert.AreEqual(expected, actual, 1e-3f);
        }

        [Test]
        public void NullConfigReturnsZero()
        {
            Vector3 v = BallisticsSolver.ComputeReleaseVelocity(null, Vector3.forward);
            Assert.AreEqual(Vector3.zero, v);
        }
    }
}
