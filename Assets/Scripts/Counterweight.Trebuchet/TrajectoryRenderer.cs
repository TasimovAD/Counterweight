using Counterweight.Core;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Drives a <see cref="LineRenderer"/> to draw a "ghost" arc of where the
    /// projectile would land under the current configuration, aim, and power.
    /// Hidden during the actual launch so the real projectile is the focus.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class TrajectoryRenderer : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private TrebuchetController controller;
        [SerializeField] private TrebuchetConfig config;
        [SerializeField] private TrebuchetAimController aimController;
        [SerializeField] private Transform releasePoint;
        [SerializeField] private ProjectileConfig projectileConfig;

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

            float power = aimController != null ? aimController.Power : 1f;
            float drag = projectileConfig != null ? projectileConfig.linearDamping : 0f;

            int count = BallisticsSimulator.SimulatePath(
                config,
                releasePoint.position,
                releasePoint.forward,
                power,
                drag,
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
                    return true;
                default:
                    return false;
            }
        }
    }
}
