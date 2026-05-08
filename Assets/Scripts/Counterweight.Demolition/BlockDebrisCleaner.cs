using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Removes detached blocks from the scene once they've come to rest, freeing
    /// physics CPU after a demolition. Also yanks any block that falls below a
    /// "void" Y level (safety net for blocks that leak through colliders).
    /// </summary>
    [RequireComponent(typeof(Block))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BlockDebrisCleaner : MonoBehaviour
    {
        [Tooltip("Seconds the block must stay asleep AND detached before being destroyed.")]
        [SerializeField, Min(1f)] private float restCleanupDelay = 30f;

        [Tooltip("Y position below which the block is destroyed instantly regardless of state.")]
        [SerializeField] private float voidY = -25f;

        private Block block;
        private Rigidbody rb;
        private float sleepingSince = -1f;

        private void Awake()
        {
            block = GetComponent<Block>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (transform.position.y < voidY)
            {
                Destroy(gameObject);
                return;
            }

            if (!block.IsDetached) return;

            if (rb.IsSleeping())
            {
                if (sleepingSince < 0f) sleepingSince = Time.time;
                if (Time.time - sleepingSince > restCleanupDelay)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                sleepingSince = -1f;
            }
        }
    }
}
