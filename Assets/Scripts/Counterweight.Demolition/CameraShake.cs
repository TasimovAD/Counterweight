using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Modulates this transform's localPosition with smooth Perlin noise to fake
    /// a camera shake. Add via static <see cref="AddImpact"/>; intensity decays.
    ///
    /// Attach to the camera (or the head pivot above it) — shake offsets are in
    /// local space, so PlayerController's yaw/pitch logic is not affected.
    /// </summary>
    public sealed class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [Tooltip("How quickly the shake intensity bleeds off (units per second).")]
        [SerializeField, Min(0.01f)] private float decay = 4f;

        [Tooltip("Hard cap on offset magnitude in meters.")]
        [SerializeField, Min(0f)] private float maxOffset = 0.25f;

        [Tooltip("Higher = more rapid micro-jitter; lower = slower, smoother sway.")]
        [SerializeField, Min(1f)] private float frequency = 22f;

        private float currentIntensity;
        private Vector3 baseLocalPosition;
        private float seedX;
        private float seedY;
        private float seedZ;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            baseLocalPosition = transform.localPosition;
            seedX = Random.value * 1000f;
            seedY = Random.value * 1000f;
            seedZ = Random.value * 1000f;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Adds to the current shake intensity (clamped to <see cref="maxOffset"/>).</summary>
        public static void AddImpact(float intensity)
        {
            if (Instance == null || intensity <= 0f) return;
            Instance.currentIntensity = Mathf.Min(Instance.maxOffset, Instance.currentIntensity + intensity);
        }

        private void LateUpdate()
        {
            if (currentIntensity <= 0.0001f)
            {
                transform.localPosition = baseLocalPosition;
                return;
            }

            float t = Time.time * frequency;
            Vector3 offset = new Vector3(
                Mathf.PerlinNoise(seedX, t) - 0.5f,
                Mathf.PerlinNoise(seedY, t) - 0.5f,
                Mathf.PerlinNoise(seedZ, t) - 0.5f) * 2f * currentIntensity;

            transform.localPosition = baseLocalPosition + offset;
            currentIntensity = Mathf.Max(0f, currentIntensity - decay * Time.deltaTime);
        }
    }
}
