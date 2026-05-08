using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Watches a <see cref="DemolitionTarget"/> for sudden stability drops and
    /// fires a "collapse" reaction: a low rumble audio clip and a camera shake
    /// proportional to how much mass dropped at once.
    /// </summary>
    [RequireComponent(typeof(DemolitionTarget))]
    public sealed class DemolitionRumble : MonoBehaviour
    {
        [SerializeField] private DemolitionEffectsConfig config;

        private DemolitionTarget target;
        private float prevStability = 1f;

        private void Awake()
        {
            target = GetComponent<DemolitionTarget>();
        }

        private void OnEnable()
        {
            target.StabilityChanged += HandleStabilityChanged;
            prevStability = target.Stability;
        }

        private void OnDisable()
        {
            target.StabilityChanged -= HandleStabilityChanged;
        }

        private void HandleStabilityChanged(float stability)
        {
            float drop = prevStability - stability;
            prevStability = stability;

            if (config == null || drop < config.collapseThreshold) return;

            float dropPercent = drop * 100f;
            float shake = Mathf.Max(config.collapseShakeMin, dropPercent * config.collapseShakePerPercent);
            CameraShake.AddImpact(shake);

            if (config.collapseRumbleClip != null && config.collapseRumbleVolume > 0f)
            {
                AudioSource.PlayClipAtPoint(config.collapseRumbleClip, transform.position, config.collapseRumbleVolume);
            }
        }
    }
}
