namespace Counterweight.Player
{
    /// <summary>
    /// Anything the player can target with a raycast and trigger via the Interact key.
    /// Implementers usually live on a GameObject with a (trigger) Collider so the
    /// raycast can hit them. The label is shown as a HUD prompt.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Short imperative text shown in the prompt, e.g. "Натянуть лебёдку".</summary>
        string PromptText { get; }

        /// <summary>True when the action is currently valid (e.g. trebuchet is in the right state).</summary>
        bool CanInteract { get; }

        /// <summary>Performs the action.</summary>
        void Interact();
    }
}
