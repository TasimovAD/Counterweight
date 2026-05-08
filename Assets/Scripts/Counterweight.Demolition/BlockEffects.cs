using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Hooks a <see cref="Block"/>'s damage/detach events to particle prefabs,
    /// audio one-shots, and camera shake. Stateless beyond holding refs;
    /// can be attached to every block in a tower.
    /// </summary>
    [RequireComponent(typeof(Block))]
    public sealed class BlockEffects : MonoBehaviour
    {
        [SerializeField] private DemolitionEffectsConfig config;

        private Block block;

        private void Awake()
        {
            block = GetComponent<Block>();
        }

        private void OnEnable()
        {
            block.Damaged += HandleDamaged;
            block.Detached += HandleDetached;
        }

        private void OnDisable()
        {
            block.Damaged -= HandleDamaged;
            block.Detached -= HandleDetached;
        }

        private void HandleDamaged(float damage, Vector3 point)
        {
            if (config == null) return;
            SpawnDust(config.impactDustPrefab, config.impactDustLifetime, point);
            PlayClip(config.impactThudClip, point, config.impactThudVolume);
            CameraShake.AddImpact(config.impactShake);
        }

        private void HandleDetached(Vector3 point)
        {
            if (config == null) return;
            SpawnDust(config.detachDustPrefab, config.detachDustLifetime, point);
            PlayClip(config.jointCrackClip, point, config.jointCrackVolume);
            CameraShake.AddImpact(config.detachShake);
        }

        private static void SpawnDust(GameObject prefab, float lifetime, Vector3 point)
        {
            if (prefab == null) return;
            GameObject inst = Instantiate(prefab, point, Quaternion.identity);
            Destroy(inst, lifetime);
        }

        private static void PlayClip(AudioClip clip, Vector3 point, float volume)
        {
            if (clip == null || volume <= 0f) return;
            AudioSource.PlayClipAtPoint(clip, point, volume);
        }
    }
}
