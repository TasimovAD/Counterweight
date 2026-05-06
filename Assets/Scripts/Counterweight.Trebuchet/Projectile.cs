using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Lifetime + first-collision hook for a spawned projectile.
    /// SFX/VFX hooks for impacts will be added in later iterations.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float lifetimeSeconds = 10f;

        private bool hasImpacted;

        private void Start()
        {
            Destroy(gameObject, lifetimeSeconds);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasImpacted) return;
            hasImpacted = true;
            Debug.Log($"[Projectile] Impact with {collision.collider.name} at {collision.GetContact(0).point:F2}.");
        }
    }
}
