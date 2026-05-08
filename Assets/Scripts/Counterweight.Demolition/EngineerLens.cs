using System;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Global toggle for the engineer's structural-vision overlay. Pure static
    /// state + event so any subscribed component (BlockLensIndicator, future
    /// stress-load arrows, etc.) reacts without lookups.
    ///
    /// The actual key binding lives in <see cref="EngineerLensInput"/>.
    /// </summary>
    public static class EngineerLens
    {
        public static bool IsActive { get; private set; }
        public static event Action<bool> ActiveChanged;

        public static void SetActive(bool active)
        {
            if (IsActive == active) return;
            IsActive = active;
            ActiveChanged?.Invoke(active);
        }

        public static void Toggle() => SetActive(!IsActive);
    }
}
