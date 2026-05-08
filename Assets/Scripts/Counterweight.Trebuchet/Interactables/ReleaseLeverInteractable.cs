using Counterweight.Player;
using UnityEngine;

namespace Counterweight.Trebuchet.Interactables
{
    /// <summary>
    /// Player-facing release lever. Available in Loaded state; pulls the trigger
    /// pin and starts the Fire animation.
    /// </summary>
    public sealed class ReleaseLeverInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private TrebuchetController controller;
        [SerializeField] private string promptText = "Дёрнуть рычаг";

        public string PromptText => promptText;
        public bool CanInteract => controller != null && controller.State == TrebuchetState.Loaded;

        public void Interact()
        {
            if (controller != null) controller.ReleaseShot();
        }
    }
}
