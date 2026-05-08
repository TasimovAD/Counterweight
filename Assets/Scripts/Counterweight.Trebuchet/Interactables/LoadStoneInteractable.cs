using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Trebuchet.Interactables
{
    /// <summary>
    /// Player-facing prompt at the projectile basket. Available in Armed state
    /// (counterweight already up). Loads a stone, advancing the trebuchet to Loaded.
    /// </summary>
    public sealed class LoadStoneInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private TrebuchetController controller;
        [SerializeField] private string promptText = "Зарядить камень";

        public string PromptText => promptText;
        public bool CanInteract => controller != null && controller.State == TrebuchetState.Armed;

        public void Interact()
        {
            if (controller != null) controller.LoadProjectile();
        }
    }
}
