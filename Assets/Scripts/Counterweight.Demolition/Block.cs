using System;
using System.Collections;
using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// One brick in a destructible structure.
    ///
    /// Two paths to fall:
    ///   - Physics: a joint connecting this block to a neighbor exceeds breakForce/breakTorque
    ///     (handled by Unity automatically; we just observe via OnJointBreak).
    ///   - Damage: small impacts accumulate, HP drops to 0, all attached joints are destroyed.
    /// Either way, once disconnected, the block falls under gravity like any free Rigidbody.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Block : MonoBehaviour
    {
        [SerializeField] private BlockConfig config;

        public float Hp { get; private set; }
        public float MaxHp { get; private set; }
        public float HpFraction => MaxHp > 0f ? Mathf.Clamp01(Hp / MaxHp) : 0f;
        public float Mass => rb != null ? rb.mass : 0f;
        public Vector3 StartingPosition { get; private set; }
        public bool IsDetached { get; private set; }

        /// <summary>Fired when an impact deals non-zero damage. Args: damage, contact point.</summary>
        public event Action<float, Vector3> Damaged;

        /// <summary>Fired exactly once when the block becomes fully detached from neighbors.</summary>
        public event Action<Vector3> Detached;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (config != null)
            {
                rb.mass = config.mass;
                MaxHp = config.maxHp;
            }
            else
            {
                MaxHp = 10f;
            }
            Hp = MaxHp;

            // Higher solver iterations = stable joint stacks. Cheap at our block counts.
            rb.solverIterations = 12;
            rb.solverVelocityIterations = 4;

            StartingPosition = transform.position;
        }

        public void ApplyJointSettingsToOwnedJoints()
        {
            if (config == null) return;
            foreach (Joint joint in GetComponents<Joint>())
            {
                joint.breakForce = config.jointBreakForce;
                joint.breakTorque = config.jointBreakTorque;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsDetached || config == null) return;

            float impulse = collision.impulse.magnitude;
            if (impulse < config.damageThreshold) return;

            float damage = (impulse - config.damageThreshold) * config.damageMultiplier;
            if (damage <= 0f) return;

            Hp = Mathf.Max(0f, Hp - damage);

            Vector3 point = collision.contactCount > 0
                ? collision.GetContact(0).point
                : transform.position;
            Damaged?.Invoke(damage, point);

            if (Hp <= 0f)
            {
                BreakAttachedJoints();
            }
        }

        private void OnJointBreak(float breakForce)
        {
            // Unity destroys the broken Joint after this callback returns, so we
            // need to wait a frame before checking how many are actually left.
            StartCoroutine(CheckDetachedAfterFrame());
        }

        private IEnumerator CheckDetachedAfterFrame()
        {
            yield return null;
            if (IsDetached) yield break;
            if (HasAnyJointLeft()) yield break;
            IsDetached = true;
            Detached?.Invoke(transform.position);
        }

        private bool HasAnyJointLeft()
        {
            foreach (Joint joint in GetComponents<Joint>())
            {
                if (joint != null) return true;
            }
            return false;
        }

        private void BreakAttachedJoints()
        {
            foreach (Joint joint in GetComponents<Joint>())
            {
                Destroy(joint);
            }
            if (!IsDetached)
            {
                IsDetached = true;
                Detached?.Invoke(transform.position);
            }
        }
    }
}
