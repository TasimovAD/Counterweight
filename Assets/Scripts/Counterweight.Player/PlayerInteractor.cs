using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Player
{
    /// <summary>
    /// Casts a ray forward from the camera every frame and tracks the nearest
    /// <see cref="IInteractable"/>. Renders a small crosshair and, when the target
    /// is interactable AND currently valid, a hint at the bottom of the screen.
    /// On Interact key (default E), invokes the interactable.
    ///
    /// OnGUI is used for the HUD because it requires no scene setup. We'll
    /// upgrade to UI Toolkit / uGUI when we add a real menu pass.
    /// </summary>
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [Header("Raycast")]
        [SerializeField] private Transform rayOrigin;
        [SerializeField, Min(0.1f)] private float maxDistance = 3f;
        [SerializeField] private LayerMask layerMask = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

        [Header("HUD")]
        [SerializeField] private bool drawCrosshair = true;
        [SerializeField] private bool drawPrompt = true;
        [SerializeField, Min(2f)] private float crosshairSize = 4f;

        private IInteractable current;
        private GUIStyle promptStyle;
        private GUIStyle promptShadowStyle;

        private void Awake()
        {
            if (rayOrigin == null)
            {
                rayOrigin = transform;
            }
        }

        private void Update()
        {
            UpdateCurrentInteractable();

            Keyboard kb = Keyboard.current;
            if (kb == null || current == null) return;

            if (kb.eKey.wasPressedThisFrame && current.CanInteract)
            {
                current.Interact();
            }
        }

        private void UpdateCurrentInteractable()
        {
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, maxDistance, layerMask, triggerInteraction))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    current = interactable;
                    return;
                }
                if (hit.collider.attachedRigidbody != null
                    && hit.collider.attachedRigidbody.TryGetComponent(out IInteractable rbInteractable))
                {
                    current = rbInteractable;
                    return;
                }
            }
            current = null;
        }

        private void EnsureStyles()
        {
            if (promptStyle != null) return;
            promptStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };
            promptStyle.normal.textColor = Color.white;
            promptShadowStyle = new GUIStyle(promptStyle);
            promptShadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
        }

        private void OnGUI()
        {
            EnsureStyles();

            if (drawCrosshair)
            {
                Rect r = new Rect(
                    (Screen.width - crosshairSize) * 0.5f,
                    (Screen.height - crosshairSize) * 0.5f,
                    crosshairSize, crosshairSize);
                GUI.color = Color.white;
                GUI.DrawTexture(r, Texture2D.whiteTexture);
            }

            if (drawPrompt && current != null && current.CanInteract)
            {
                string text = $"[E]  {current.PromptText}";
                float w = 600f;
                float h = 40f;
                Rect pr = new Rect((Screen.width - w) * 0.5f, Screen.height - h - 80f, w, h);
                Rect shadow = new Rect(pr.x + 2f, pr.y + 2f, pr.width, pr.height);
                GUI.Label(shadow, text, promptShadowStyle);
                GUI.Label(pr, text, promptStyle);
            }
        }
    }
}
