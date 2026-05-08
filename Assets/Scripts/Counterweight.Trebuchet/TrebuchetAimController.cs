using UnityEngine;
using UnityEngine.InputSystem;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Player-driven horizontal aiming and power tuning for the trebuchet.
    /// Rotates the GameObject this is attached to around the world Y axis
    /// (so the rest of the trebuchet — including the release point — turns
    /// with it), and exposes a 0..N power multiplier consumed by the
    /// solver/simulator/spawner.
    ///
    /// Default bindings (placeholder, will move into InputSystem_Actions later):
    ///   Left / Right arrow  — rotate aim
    ///   Up / Down arrow     — adjust power continuously
    ///   Mouse scroll wheel  — adjust power in steps
    /// </summary>
    public sealed class TrebuchetAimController : MonoBehaviour
    {
        [Header("Aim")]
        [SerializeField, Min(1f)] private float aimSpeedDegPerSec = 45f;
        [SerializeField] private float minYawDeg = -90f;
        [SerializeField] private float maxYawDeg = 90f;

        [Header("Power")]
        [SerializeField, Range(0.05f, 2f)] private float minPower = 0.3f;
        [SerializeField, Range(0.1f, 3f)] private float maxPower = 1.5f;
        [SerializeField, Range(0.1f, 3f)] private float startingPower = 1f;
        [SerializeField, Min(0.05f)] private float keyPowerSpeed = 0.5f;
        [SerializeField, Min(0.0001f)] private float scrollPowerStep = 0.001f;

        public float Power { get; private set; } = 1f;
        public float Yaw { get; private set; }

        private Quaternion baseRotation;

        private void Awake()
        {
            Power = Mathf.Clamp(startingPower, minPower, maxPower);
        }

        private void Start()
        {
            baseRotation = transform.localRotation;
            Yaw = 0f;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (keyboard != null)
            {
                float aimInput = 0f;
                if (keyboard.leftArrowKey.isPressed) aimInput -= 1f;
                if (keyboard.rightArrowKey.isPressed) aimInput += 1f;
                if (Mathf.Abs(aimInput) > 0f)
                {
                    Yaw = Mathf.Clamp(Yaw + aimInput * aimSpeedDegPerSec * Time.deltaTime, minYawDeg, maxYawDeg);
                    transform.localRotation = baseRotation * Quaternion.Euler(0f, Yaw, 0f);
                }

                float powerInput = 0f;
                if (keyboard.upArrowKey.isPressed) powerInput += 1f;
                if (keyboard.downArrowKey.isPressed) powerInput -= 1f;
                if (Mathf.Abs(powerInput) > 0f)
                {
                    Power = Mathf.Clamp(Power + powerInput * keyPowerSpeed * Time.deltaTime, minPower, maxPower);
                }
            }

            if (mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    Power = Mathf.Clamp(Power + scroll * scrollPowerStep, minPower, maxPower);
                }
            }
        }
    }
}
