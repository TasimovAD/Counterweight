using Counterweight.Core;
using NUnit.Framework;
using UnityEngine;

namespace Counterweight.Tests
{
    public sealed class BallisticsSimulatorTests
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
        public void StartsAtOrigin()
        {
            var cfg = MakeConfig();
            var buf = new Vector3[64];
            Vector3 origin = new Vector3(1f, 5f, 2f);

            int count = BallisticsSimulator.SimulatePath(cfg, DefaultProjectileMass, origin, Vector3.forward, 1f, 0f, buf);

            Assert.Greater(count, 1);
            Assert.AreEqual(origin, buf[0]);
        }

        [Test]
        public void TrajectoryRisesThenFalls()
        {
            var cfg = MakeConfig();
            var buf = new Vector3[256];

            int count = BallisticsSimulator.SimulatePath(cfg, DefaultProjectileMass, Vector3.zero, Vector3.forward, 1f, 0f, buf, 0.05f);

            float maxY = float.MinValue;
            int apexIndex = -1;
            for (int i = 0; i < count; i++)
            {
                if (buf[i].y > maxY)
                {
                    maxY = buf[i].y;
                    apexIndex = i;
                }
            }

            Assert.Greater(maxY, 0f, "trajectory should reach a positive apex");
            Assert.Greater(apexIndex, 0, "apex should not be at the start");
            Assert.Less(apexIndex, count - 1, "apex should not be at the end (the path should descend after)");
            Assert.Less(buf[count - 1].y, maxY, "final point should be below apex");
        }

        [Test]
        public void HigherPowerLandsFartherWithoutDrag()
        {
            var cfg = MakeConfig();
            var bufA = new Vector3[1024];
            var bufB = new Vector3[1024];

            int countA = BallisticsSimulator.SimulatePath(cfg, DefaultProjectileMass, Vector3.zero, Vector3.forward, 0.5f, 0f, bufA, 0.05f, 0f);
            int countB = BallisticsSimulator.SimulatePath(cfg, DefaultProjectileMass, Vector3.zero, Vector3.forward, 1.5f, 0f, bufB, 0.05f, 0f);

            float distA = bufA[countA - 1].magnitude;
            float distB = bufB[countB - 1].magnitude;

            Assert.Greater(distB, distA);
        }

        [Test]
        public void StopsAtGround()
        {
            var cfg = MakeConfig();
            var buf = new Vector3[1024];

            int count = BallisticsSimulator.SimulatePath(cfg, DefaultProjectileMass, new Vector3(0f, 5f, 0f), Vector3.forward, 1f, 0f, buf, 0.05f, 0f);

            Assert.Greater(count, 1);
            Assert.LessOrEqual(buf[count - 1].y, 0f);
            Assert.Greater(buf[count - 2].y, 0f);
        }

        [Test]
        public void NullConfigReturnsZero()
        {
            var buf = new Vector3[16];
            int count = BallisticsSimulator.SimulatePath(null, DefaultProjectileMass, Vector3.zero, Vector3.forward, 1f, 0f, buf);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void EmptyBufferReturnsZero()
        {
            var cfg = MakeConfig();
            var buf = new Vector3[0];
            int count = BallisticsSimulator.SimulatePath(cfg, DefaultProjectileMass, Vector3.zero, Vector3.forward, 1f, 0f, buf);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void LighterProjectileFliesFurther()
        {
            var cfg = MakeConfig();
            var bufHeavy = new Vector3[1024];
            var bufLight = new Vector3[1024];

            int countHeavy = BallisticsSimulator.SimulatePath(cfg, projectileMass: 16f, Vector3.zero, Vector3.forward, 1f, 0f, bufHeavy, 0.05f, 0f);
            int countLight = BallisticsSimulator.SimulatePath(cfg, projectileMass: 4f, Vector3.zero, Vector3.forward, 1f, 0f, bufLight, 0.05f, 0f);

            float distHeavy = bufHeavy[countHeavy - 1].magnitude;
            float distLight = bufLight[countLight - 1].magnitude;

            Assert.Greater(distLight, distHeavy);
        }
    }
}
