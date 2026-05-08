using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Trebuchet.Interactables
{
    /// <summary>
    /// Player-facing handle on the trebuchet's winch. Available in Idle state;
    /// triggers the wind-up animation.
    /// </summary>
    public sealed class WinchInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private TrebuchetController controller;
        [SerializeField] private string promptText = "Натянуть лебёдку";

        public string PromptText => promptText;
        public bool CanInteract => controller != null && controller.State == TrebuchetState.Idle;

        public void Interact()
        {
            if (controller != null) controller.BeginWindUp();
        }
    }
}
