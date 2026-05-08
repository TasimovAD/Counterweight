using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Tiny OnGUI overlay showing the current stability of a target.
    /// One instance per target, stacked vertically when there are several.
    /// </summary>
    public sealed class DemolitionHUD : MonoBehaviour
    {
        [SerializeField] private DemolitionTarget target;
        [SerializeField] private string label = "Стабильность";
        [SerializeField] private Vector2 screenOffset = new(20f, 20f);

        private GUIStyle textStyle;
        private GUIStyle shadowStyle;

        private void EnsureStyles()
        {
            if (textStyle != null) return;
            textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft,
            };
            textStyle.normal.textColor = Color.white;

            shadowStyle = new GUIStyle(textStyle);
            shadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
        }

        private void OnGUI()
        {
            if (target == null) return;
            EnsureStyles();

            string text = target.IsComplete
                ? $"{label}: 0% — Демонтаж завершён"
                : $"{label}: {Mathf.RoundToInt(target.Stability * 100f)}%";

            Rect rect = new Rect(screenOffset.x, screenOffset.y, 500f, 32f);
            Rect shadow = new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height);
            GUI.Label(shadow, text, shadowStyle);
            GUI.Label(rect, text, textStyle);
        }
    }
}
