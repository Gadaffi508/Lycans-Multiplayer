using UnityEngine;

namespace Mirror
{
    public class CameraController : MonoBehaviour
    {
        private const float YMin = -50f;
        private const float YMax = 50f;

        public Transform lookAt;
        public float distance;
        public float sensitivity;

        private float currentX = 0;
        private float currentY = 25;

        private bool _clickEscape = false;

        private void LateUpdate()
        {
            currentX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            currentY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            
            currentY = Mathf.Clamp(currentY, YMin, YMax);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
            Vector3 direction = new Vector3(0, 0, -distance);
            transform.position = lookAt.position + rotation * direction;

            transform.LookAt(lookAt.position);

            if (_clickEscape is true)
                Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;

            if (Input.GetKeyDown(KeyCode.Escape))
                _clickEscape = !_clickEscape;
        }
    }
}