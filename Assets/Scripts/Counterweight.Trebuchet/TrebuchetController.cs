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
    /// State flow for the MVP:
    ///   Idle --(RequestFire)--> Firing --(animation event)--> Released --(timer)--> Idle
    /// </summary>
    public sealed class TrebuchetController : MonoBehaviour
    {
        private static readonly int FireTrigger = Animator.StringToHash("Fire");

        [Header("Configuration")]
        [SerializeField] private TrebuchetConfig config;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform releasePoint;
        [SerializeField] private ProjectileSpawner spawner;
        [SerializeField] private TrebuchetAnimationRelay animationRelay;

        [Header("Tuning")]
        [SerializeField, Min(0.1f)] private float resetDelaySeconds = 3f;

        public TrebuchetState State { get; private set; } = TrebuchetState.Idle;

        private void OnEnable()
        {
            if (animationRelay != null)
            {
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
            if (State != TrebuchetState.Idle)
            {
                return;
            }
            if (animator == null)
            {
                Debug.LogError("[TrebuchetController] Animator is not assigned.", this);
                return;
            }

            State = TrebuchetState.Firing;
            animator.SetTrigger(FireTrigger);
        }

        private void HandleProjectileRelease()
        {
            if (State != TrebuchetState.Firing)
            {
                return;
            }

            if (spawner != null)
            {
                spawner.Spawn(config, releasePoint);
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
