using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Placeholder free-fly camera for the firing range scene.
    /// Right mouse button to look, WASD/QE to move, Shift to sprint.
    /// Uses legacy <see cref="UnityEngine.Input"/> directly because it is
    /// scaffolding and will be replaced by an FPV character controller later.
    /// </summary>
    public sealed class DebugFlyCamera : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float sprintMultiplier = 3f;
        [SerializeField] private float lookSensitivity = 2f;

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
            if (UnityEngine.Input.GetMouseButton(1))
            {
                yaw += UnityEngine.Input.GetAxis("Mouse X") * lookSensitivity;
                pitch -= UnityEngine.Input.GetAxis("Mouse Y") * lookSensitivity;
                pitch = Mathf.Clamp(pitch, -89f, 89f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            Vector3 dir = Vector3.zero;
            if (UnityEngine.Input.GetKey(KeyCode.W)) dir += transform.forward;
            if (UnityEngine.Input.GetKey(KeyCode.S)) dir -= transform.forward;
            if (UnityEngine.Input.GetKey(KeyCode.A)) dir -= transform.right;
            if (UnityEngine.Input.GetKey(KeyCode.D)) dir += transform.right;
            if (UnityEngine.Input.GetKey(KeyCode.E)) dir += Vector3.up;
            if (UnityEngine.Input.GetKey(KeyCode.Q)) dir -= Vector3.up;

            float speed = moveSpeed * (UnityEngine.Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
            transform.position += dir * (speed * Time.deltaTime);
        }
    }
}
