using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Watches Tab and toggles the global <see cref="EngineerLens"/> state.
    /// Drop one instance into a scene (or the player rig). HUD shows a small
    /// hint when the lens is active.
    /// </summary>
    public sealed class EngineerLensInput : MonoBehaviour
    {
        [SerializeField] private bool drawHint = true;

        private GUIStyle hintStyle;
        private GUIStyle hintShadow;

        private void Update()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return;
            if (kb.tabKey.wasPressedThisFrame)
            {
                EngineerLens.Toggle();
            }
        }

        private void EnsureStyles()
        {
            if (hintStyle != null) return;
            hintStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.LowerLeft };
            hintStyle.normal.textColor = new Color(0.7f, 1f, 0.8f, 0.9f);
            hintShadow = new GUIStyle(hintStyle);
            hintShadow.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
        }

        private void OnGUI()
        {
            if (!drawHint) return;
            EnsureStyles();
            string text = EngineerLens.IsActive ? "[Tab] Лупа инженера: вкл" : "[Tab] Лупа инженера: выкл";
            Rect rect = new Rect(20f, Screen.height - 50f, 300f, 24f);
            Rect shadow = new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height);
            GUI.Label(shadow, text, hintShadow);
            GUI.Label(rect, text, hintStyle);
        }
    }
}
