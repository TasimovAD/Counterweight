using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Player
{
    /// <summary>
    /// First-person walk-and-look controller tuned for a cozy game:
    /// - No jumping, no crouching, no head bob.
    /// - Slow comfortable walk; gentle sprint with Shift.
    /// - Mouse look with cursor lock; Escape unlocks, click to relock.
    ///
    /// Uses the new Input System directly (Keyboard.current / Mouse.current)
    /// because we don't need the configurable bindings of an action asset yet.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float sprintMultiplier = 1.6f;
        [SerializeField] private float gravity = -19.62f;

        [Header("Look")]
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float lookSensitivity = 0.12f;
        [SerializeField] private float minPitchDeg = -85f;
        [SerializeField] private float maxPitchDeg = 85f;

        private CharacterController controller;
        private float yaw;
        private float pitch;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraPivot == null && Camera.main != null)
            {
                cameraPivot = Camera.main.transform;
            }

            yaw = transform.eulerAngles.y;
            pitch = cameraPivot != null ? cameraPivot.localEulerAngles.x : 0f;

            LockCursor();
        }

        private void Update()
        {
            HandleCursor();
            HandleLook();
            HandleMovement();
        }

        private void HandleCursor()
        {
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
        }

        private void HandleLook()
        {
            if (Cursor.lockState != CursorLockMode.Locked || cameraPivot == null) return;
            Mouse mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * lookSensitivity;
            pitch -= delta.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, minPitchDeg, maxPitchDeg);

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            Keyboard kb = Keyboard.current;
            Vector3 input = Vector3.zero;
            if (kb != null)
            {
                if (kb.wKey.isPressed) input.z += 1f;
                if (kb.sKey.isPressed) input.z -= 1f;
                if (kb.aKey.isPressed) input.x -= 1f;
                if (kb.dKey.isPressed) input.x += 1f;
            }

            Vector3 horizontal = transform.right * input.x + transform.forward * input.z;
            if (horizontal.sqrMagnitude > 1f) horizontal.Normalize();

            float speed = walkSpeed;
            if (kb != null && kb.leftShiftKey.isPressed) speed *= sprintMultiplier;
            horizontal *= speed;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 motion = horizontal + Vector3.up * verticalVelocity;
            controller.Move(motion * Time.deltaTime);
        }

        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
