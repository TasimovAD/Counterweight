using System.Collections;
using Counterweight.Core;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Owns trebuchet state, drives the Animator, and tells the spawner
    /// when the animation event fires.
    ///
    /// State flow (advanced via player interactions, see Counterweight.Player):
    ///   Idle      --(BeginWindUp)----> WindingUp --(anim event)--> Armed
    ///   Armed     --(LoadProjectile)-> Loaded   (a specific ProjectileConfig is captured here)
    ///   Loaded    --(ReleaseShot)----> Firing    --(anim event)--> Released
    ///   Released  --(timer)---------->  Idle
    /// </summary>
    public sealed class TrebuchetController : MonoBehaviour
    {
        private static readonly int WindUpTrigger = Animator.StringToHash("WindUp");
        private static readonly int FireTrigger = Animator.StringToHash("Fire");

        [Header("Configuration")]
        [SerializeField] private TrebuchetConfig config;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform releasePoint;
        [SerializeField] private ProjectileSpawner spawner;
        [SerializeField] private TrebuchetAnimationRelay animationRelay;
        [SerializeField] private TrebuchetAimController aimController;

        [Header("Tuning")]
        [SerializeField, Min(0.1f)] private float resetDelaySeconds = 3f;

        public TrebuchetState State { get; private set; } = TrebuchetState.Idle;
        public ProjectileConfig LoadedProjectile { get; private set; }

        private void OnEnable()
        {
            if (animationRelay != null)
            {
                animationRelay.WindUpComplete += HandleWindUpComplete;
                animationRelay.ProjectileRelease += HandleProjectileRelease;
            }
        }

        private void OnDisable()
        {
            if (animationRelay != null)
            {
                animationRelay.WindUpComplete -= HandleWindUpComplete;
                animationRelay.ProjectileRelease -= HandleProjectileRelease;
            }
        }

        public void BeginWindUp()
        {
            if (State != TrebuchetState.Idle) return;
            if (animator == null)
            {
                Debug.LogError("[TrebuchetController] Animator is not assigned.", this);
                return;
            }
            State = TrebuchetState.WindingUp;
            animator.SetTrigger(WindUpTrigger);
        }

        public void LoadProjectile(ProjectileConfig projectileConfig)
        {
            if (State != TrebuchetState.Armed) return;
            LoadedProjectile = projectileConfig;
            State = TrebuchetState.Loaded;
        }

        public void ReleaseShot()
        {
            if (State != TrebuchetState.Loaded) return;
            if (animator == null)
            {
                Debug.LogError("[TrebuchetController] Animator is not assigned.", this);
                return;
            }
            State = TrebuchetState.Firing;
            animator.SetTrigger(FireTrigger);
        }

        private void HandleWindUpComplete()
        {
            if (State != TrebuchetState.WindingUp) return;
            State = TrebuchetState.Armed;
        }

        private void HandleProjectileRelease()
        {
            if (State != TrebuchetState.Firing) return;

            if (spawner != null)
            {
                float power = aimController != null ? aimController.Power : 1f;
                ProjectileConfig pc = LoadedProjectile != null ? LoadedProjectile : spawner.DefaultProjectileConfig;
                spawner.Spawn(config, pc, releasePoint, power);
            }

            LoadedProjectile = null;
            State = TrebuchetState.Released;
            StartCoroutine(ResetAfterDelay());
        }

        private IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(resetDelaySeconds);
            State = TrebuchetState.Idle;
        }
    }
}
