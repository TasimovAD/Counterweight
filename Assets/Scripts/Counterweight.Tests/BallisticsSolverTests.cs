using Counterweight.Core;
using NUnit.Framework;
using UnityEngine;

namespace Counterweight.Tests
{
    public sealed class BallisticsSolverTests
    {
        private const float DefaultProjectileMass = 8f;

        private static TrebuchetConfig MakeConfig(
            float counterweightMass = 200f,
            float armLength = 4f,
            float releaseAngleDeg = 45f,
            float launchEfficiency = 0.55f)
        {
            var cfg = ScriptableObject.CreateInstance<TrebuchetConfig>();
            cfg.counterweightMass = counterweightMass;
            cfg.armLength = armLength;
            cfg.releaseAngleDeg = releaseAngleDeg;
            cfg.launchEfficiency = launchEfficiency;
            return cfg;
        }

        [Test]
        public void HeavierCounterweightIncreasesSpeed()
        {
            var light = MakeConfig(counterweightMass: 100f);
            var heavy = MakeConfig(counterweightMass: 400f);

            float vLight = BallisticsSolver.ComputeReleaseSpeed(light, DefaultProjectileMass);
            float vHeavy = BallisticsSolver.ComputeReleaseSpeed(heavy, DefaultProjectileMass);

            Assert.Greater(vHeavy, vLight);
        }

        [Test]
        public void HeavierProjectileDecreasesSpeed()
        {
            var cfg = MakeConfig();

            float vLight = BallisticsSolver.ComputeReleaseSpeed(cfg, projectileMass: 4f);
            float vHeavy = BallisticsSolver.ComputeReleaseSpeed(cfg, projectileMass: 16f);

            Assert.Greater(vLight, vHeavy);
        }

        [Test]
        public void ZeroEfficiencyProducesZeroSpeed()
        {
            var cfg = MakeConfig(launchEfficiency: 0f);
            float v = BallisticsSolver.ComputeReleaseSpeed(cfg, DefaultProjectileMass);
            Assert.AreEqual(0f, v, 1e-6f);
        }

        [Test]
        public void ReleaseVelocityHasPositiveYWhenAngleIsPositive()
        {
            var cfg = MakeConfig(releaseAngleDeg: 45f);
            Vector3 v = BallisticsSolver.ComputeReleaseVelocity(cfg, DefaultProjectileMass, Vector3.forward);
            Assert.Greater(v.y, 0f);
        }

        [Test]
        public void ReleaseSpeedMatchesEnergyEquation()
        {
            // PE = m * g * h = 200 * 9.81 * 4 = 7848 J
            // KE = 0.55 * 7848 = 4316.4 J
            // v = sqrt(2 * 4316.4 / 8) = sqrt(1079.1) ~ 32.85 m/s
            var cfg = MakeConfig();
            float expected = Mathf.Sqrt(2f * (200f * 9.81f * 4f) * 0.55f / DefaultProjectileMass);
            float actual = BallisticsSolver.ComputeReleaseSpeed(cfg, DefaultProjectileMass);
            Assert.AreEqual(expected, actual, 1e-3f);
        }

        [Test]
        public void NullConfigReturnsZero()
        {
            Vector3 v = BallisticsSolver.ComputeReleaseVelocity(null, DefaultProjectileMass, Vector3.forward);
            Assert.AreEqual(Vector3.zero, v);
        }

        [Test]
        public void HigherPowerIncreasesSpeed()
        {
            var cfg = MakeConfig();
            float low = BallisticsSolver.ComputeReleaseSpeed(cfg, DefaultProjectileMass, powerMultiplier: 0.5f);
            float high = BallisticsSolver.ComputeReleaseSpeed(cfg, DefaultProjectileMass, powerMultiplier: 1.5f);
            Assert.Greater(high, low);
        }
    }
}
