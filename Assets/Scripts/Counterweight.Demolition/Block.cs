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
        public float Mass => rb != null ? rb.mass : 0f;
        public Vector3 StartingPosition { get; private set; }
        public bool IsDetached { get; private set; }

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (config != null)
            {
                rb.mass = config.mass;
                Hp = config.maxHp;
            }
            else
            {
                Hp = 10f;
            }

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
            Hp = Mathf.Max(0f, Hp - damage);
            if (Hp <= 0f)
            {
                BreakAttachedJoints();
            }
        }

        private void OnJointBreak(float breakForce)
        {
            // Unity destroys the broken Joint automatically. We only need to mark
            // the block as detached if it has no joints left.
            if (HasAnyJointLeft()) return;
            IsDetached = true;
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
            IsDetached = true;
        }
    }
}
