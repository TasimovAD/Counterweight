using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Placeholder free-fly camera for the firing range scene.
    /// Right mouse button to look, WASD/QE to move, Shift to sprint.
    /// Uses the new Input System (project has legacy input disabled).
    /// Will be replaced by an FPV character controller in a later iteration.
    /// </summary>
    public sealed class DebugFlyCamera : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float sprintMultiplier = 3f;
        [SerializeField] private float lookSensitivity = 0.15f;

        private float yaw;
        private float pitch;

        private void Start()
        {
            Vector3 e = transform.eulerAngles;
            yaw = e.y;
            pitch = e.x;
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;
            if (mouse == null || keyboard == null)
            {
                return;
            }

            if (mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                yaw += delta.x * lookSensitivity;
                pitch -= delta.y * lookSensitivity;
                pitch = Mathf.Clamp(pitch, -89f, 89f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            Vector3 dir = Vector3.zero;
            if (keyboard.wKey.isPressed) dir += transform.forward;
            if (keyboard.sKey.isPressed) dir -= transform.forward;
            if (keyboard.aKey.isPressed) dir -= transform.right;
            if (keyboard.dKey.isPressed) dir += transform.right;
            if (keyboard.eKey.isPressed) dir += Vector3.up;
            if (keyboard.qKey.isPressed) dir -= Vector3.up;

            float speed = moveSpeed * (keyboard.leftShiftKey.isPressed ? sprintMultiplier : 1f);
            transform.position += dir * (speed * Time.deltaTime);
        }
    }
}
