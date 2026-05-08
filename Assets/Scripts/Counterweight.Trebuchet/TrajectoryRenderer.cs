using Counterweight.Core;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Drives a <see cref="LineRenderer"/> to draw a "ghost" arc of where the
    /// projectile would land under the current configuration, aim, and power.
    /// Hidden during the actual launch so the real projectile is the focus.
    ///
    /// Projectile mass for the simulation is read from the active selection
    /// (<see cref="ProjectileSelector"/>) when available, falling back to the
    /// spawner's default config.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class TrajectoryRenderer : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private TrebuchetController controller;
        [SerializeField] private TrebuchetConfig config;
        [SerializeField] private TrebuchetAimController aimController;
        [SerializeField] private Transform releasePoint;
        [SerializeField] private ProjectileSelector projectileSelector;
        [SerializeField] private ProjectileConfig fallbackProjectileConfig;

        [Header("Simulation")]
        [SerializeField] private float groundY = 0f;
        [SerializeField, Min(0.01f)] private float dt = 0.05f;
        [SerializeField, Range(2, 1024)] private int maxPoints = 200;

        private LineRenderer lineRenderer;
        private Vector3[] buffer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            buffer = new Vector3[maxPoints];
        }

        private void OnValidate()
        {
            if (buffer == null || buffer.Length != maxPoints)
            {
                buffer = new Vector3[maxPoints];
            }
        }

        private void LateUpdate()
        {
            if (config == null || releasePoint == null)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            if (!ShouldShow())
            {
                lineRenderer.positionCount = 0;
                return;
            }

            ProjectileConfig pc = projectileSelector != null ? projectileSelector.Current : null;
            if (pc == null) pc = fallbackProjectileConfig;
            if (pc == null)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            float power = aimController != null ? aimController.Power : 1f;

            int count = BallisticsSimulator.SimulatePath(
                config,
                pc.mass,
                releasePoint.position,
                releasePoint.forward,
                power,
                pc.linearDamping,
                buffer,
                dt,
                groundY);

            lineRenderer.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                lineRenderer.SetPosition(i, buffer[i]);
            }
        }

        private bool ShouldShow()
        {
            if (controller == null) return true;
            switch (controller.State)
            {
                case TrebuchetState.Idle:
                case TrebuchetState.WindingUp:
                case TrebuchetState.Armed:
                case TrebuchetState.Loaded:
                    return true;
                default:
                    return false;
            }
        }
    }
}
