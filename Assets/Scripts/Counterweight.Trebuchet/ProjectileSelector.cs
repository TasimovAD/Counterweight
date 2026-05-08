using System;
using System.Collections.Generic;
using Counterweight.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Holds the list of projectile types the player can choose between
    /// (e.g. stone / clay pot / wooden ball) and exposes the current pick.
    /// Cycle bindings: 1 / 2 / 3.
    ///
    /// Locks while the trebuchet is in <see cref="TrebuchetState.Loaded"/> so
    /// the player can't switch ammo mid-shot — what's loaded is what fires.
    /// </summary>
    public sealed class ProjectileSelector : MonoBehaviour
    {
        [SerializeField] private List<ProjectileConfig> configs = new();
        [SerializeField] private TrebuchetController lockSource;

        public event Action<ProjectileConfig> SelectionChanged;

        public int CurrentIndex { get; private set; }
        public ProjectileConfig Current => configs.Count > 0 ? configs[Mathf.Clamp(CurrentIndex, 0, configs.Count - 1)] : null;
        public IReadOnlyList<ProjectileConfig> Configs => configs;

        private void Update()
        {
            if (lockSource != null && lockSource.State == TrebuchetState.Loaded) return;

            Keyboard kb = Keyboard.current;
            if (kb == null) return;

            if (kb.digit1Key.wasPressedThisFrame) Select(0);
            else if (kb.digit2Key.wasPressedThisFrame) Select(1);
            else if (kb.digit3Key.wasPressedThisFrame) Select(2);
        }

        public void Select(int index)
        {
            if (index < 0 || index >= configs.Count) return;
            if (CurrentIndex == index) return;
            CurrentIndex = index;
            SelectionChanged?.Invoke(Current);
        }
    }
}
