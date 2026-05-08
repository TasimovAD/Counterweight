using System.Collections;
using Counterweight.Core;
using Counterweight.Input;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Owns trebuchet state, listens for Fire input, drives the Animator,
    /// and tells the spawner when the animation event fires.
    ///
    /// Two-stage interaction:
    ///   Idle      --(Fire pressed)--> WindingUp --(anim event)--> Armed
    ///   Armed     --(Fire pressed)--> Firing    --(anim event)--> Released
    ///   Released  --(timer)-------->  Idle
    ///
    /// The same input button advances the trebuchet through the active states.
    /// Presses during WindingUp/Firing/Released are ignored.
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

        private void OnEnable()
        {
            if (animationRelay != null)
            {
                animationRelay.WindUpComplete += HandleWindUpComplete;
                animationRelay.ProjectileRelease += HandleProjectileRelease;
            }
            if (InputBridge.Instance != null)
            {
                InputBridge.Instance.FirePressed += RequestFire;
            }
        }

        private void OnDisable()
        {
            if (animationRelay != null)
            {
                animationRelay.WindUpComplete -= HandleWindUpComplete;
                animationRelay.ProjectileRelease -= HandleProjectileRelease;
            }
            if (InputBridge.Instance != null)
            {
                InputBridge.Instance.FirePressed -= RequestFire;
            }
        }

        private void Start()
        {
            // InputBridge may have been spawned after this controller; subscribe lazily if needed.
            if (InputBridge.Instance != null)
            {
                InputBridge.Instance.FirePressed -= RequestFire;
                InputBridge.Instance.FirePressed += RequestFire;
            }
        }

        public void RequestFire()
        {
            if (animator == null)
            {
                Debug.LogError("[TrebuchetController] Animator is not assigned.", this);
                return;
            }

            switch (State)
            {
                case TrebuchetState.Idle:
                    State = TrebuchetState.WindingUp;
                    animator.SetTrigger(WindUpTrigger);
                    break;

                case TrebuchetState.Armed:
                    State = TrebuchetState.Firing;
                    animator.SetTrigger(FireTrigger);
                    break;

                // WindingUp / Firing / Released / Resetting -> input ignored.
            }
        }

        private void HandleWindUpComplete()
        {
            if (State != TrebuchetState.WindingUp)
            {
                return;
            }
            State = TrebuchetState.Armed;
        }

        private void HandleProjectileRelease()
        {
            if (State != TrebuchetState.Firing)
            {
                return;
            }

            if (spawner != null)
            {
                float power = aimController != null ? aimController.Power : 1f;
                spawner.Spawn(config, releasePoint, power);
            }

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
