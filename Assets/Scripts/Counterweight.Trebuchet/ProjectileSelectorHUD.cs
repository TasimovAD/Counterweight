using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Top-right OnGUI strip listing available projectile types. Highlights the current pick
    /// and shows the [1/2/3] hotkeys.
    /// </summary>
    public sealed class ProjectileSelectorHUD : MonoBehaviour
    {
        [SerializeField] private ProjectileSelector selector;
        [SerializeField] private Vector2 screenOffset = new(20f, 20f);

        private GUIStyle textStyle;
        private GUIStyle textStyleActive;
        private GUIStyle shadowStyle;

        private void EnsureStyles()
        {
            if (textStyle != null) return;
            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.UpperRight };
            textStyle.normal.textColor = new Color(1f, 1f, 1f, 0.6f);

            textStyleActive = new GUIStyle(textStyle) { fontStyle = FontStyle.Bold };
            textStyleActive.normal.textColor = Color.white;

            shadowStyle = new GUIStyle(textStyle);
            shadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
        }

        private void OnGUI()
        {
            if (selector == null || selector.Configs.Count == 0) return;
            EnsureStyles();

            float lineHeight = 26f;
            float width = 280f;
            float x = Screen.width - width - screenOffset.x;
            float y = screenOffset.y;

            for (int i = 0; i < selector.Configs.Count; i++)
            {
                var cfg = selector.Configs[i];
                if (cfg == null) continue;
                bool isCurrent = i == selector.CurrentIndex;
                string text = $"[{i + 1}] {cfg.name}{(isCurrent ? "  ●" : "")}";

                Rect rect = new Rect(x, y + i * lineHeight, width, lineHeight);
                Rect shadow = new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height);
                GUI.Label(shadow, text, shadowStyle);
                GUI.Label(rect, text, isCurrent ? textStyleActive : textStyle);
            }
        }
    }
}
