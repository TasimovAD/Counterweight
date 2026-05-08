using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Bundles the VFX/SFX assets used by <see cref="BlockEffects"/> and
    /// <see cref="DemolitionRumble"/>. One config is shared by all blocks of a
    /// given material (e.g. stone, wood). Any field can be left null — that
    /// effect just won't play.
    /// </summary>
    [CreateAssetMenu(menuName = "Counterweight/Demolition Effects Config", fileName = "DemolitionEffectsConfig")]
    public sealed class DemolitionEffectsConfig : ScriptableObject
    {
        [Header("Particles (auto-destroy after Lifetime sec)")]
        public GameObject impactDustPrefab;
        public GameObject detachDustPrefab;
        [Min(0.5f)] public float impactDustLifetime = 2f;
        [Min(0.5f)] public float detachDustLifetime = 4f;

        [Header("Audio one-shots")]
        public AudioClip impactThudClip;
        [Range(0f, 1f)] public float impactThudVolume = 0.4f;
        public AudioClip jointCrackClip;
        [Range(0f, 1f)] public float jointCrackVolume = 0.6f;
        public AudioClip collapseRumbleClip;
        [Range(0f, 1f)] public float collapseRumbleVolume = 0.7f;

        [Header("Camera shake intensity")]
        [Min(0f)] public float impactShake = 0.0f;            // small or none for individual hits
        [Min(0f)] public float detachShake = 0.04f;           // per-block detach
        [Min(0f)] public float collapseShakePerPercent = 0.01f; // multiplied by % stability lost
        [Min(0f)] public float collapseShakeMin = 0.05f;      // minimum jolt per collapse trigger

        [Header("Collapse trigger")]
        [Tooltip("Stability drop within one poll interval that counts as a collapse rather than a single block.")]
        [Range(0f, 0.5f)] public float collapseThreshold = 0.04f;
    }
}
