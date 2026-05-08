using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Trebuchet.Interactables
{
    /// <summary>
    /// Player-facing prompt at the projectile basket. Available in Armed state
    /// (counterweight already up). Loads the projectile currently selected in the
    /// optional <see cref="ProjectileSelector"/>; falls back to the spawner's default.
    /// </summary>
    public sealed class LoadStoneInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private TrebuchetController controller;
        [SerializeField] private ProjectileSelector selector;
        [SerializeField] private string promptText = "Зарядить снаряд";

        public string PromptText => promptText;
        public bool CanInteract => controller != null && controller.State == TrebuchetState.Armed;

        public void Interact()
        {
            if (controller == null) return;
            controller.LoadProjectile(selector != null ? selector.Current : null);
        }
    }
}
