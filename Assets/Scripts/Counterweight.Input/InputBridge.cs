using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Input
{
    /// <summary>
    /// Minimal wrapper around the project's generated <c>InputSystem_Actions</c> C# class.
    /// Reuses the existing <c>Player/Attack</c> action as the Fire button for the trebuchet MVP.
    ///
    /// Editor setup:
    ///   1. Select InputSystem_Actions.inputactions, enable "Generate C# Class", apply.
    ///      This produces an <c>InputSystem_Actions</c> partial class used below.
    ///   2. Drop a single InputBridge instance into the scene.
    /// </summary>
    public sealed class InputBridge : MonoBehaviour
    {
        public static InputBridge Instance { get; private set; }

        public event Action FirePressed;

        private InputSystem_Actions actions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            actions.Player.Enable();
            actions.Player.Attack.performed += OnAttackPerformed;
        }

        private void OnDisable()
        {
            actions.Player.Attack.performed -= OnAttackPerformed;
            actions.Player.Disable();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            actions?.Dispose();
        }

        private void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            FirePressed?.Invoke();
        }
    }
}
