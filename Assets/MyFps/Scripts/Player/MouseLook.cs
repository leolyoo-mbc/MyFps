using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(CharacterInput))]
    public class MouseLook : MonoBehaviour
    {
        #region Variables
        public Transform cameraRoot;
        private CharacterInput input;

        [SerializeField] private float rotationSpeed = 1.0f;
        [SerializeField] private float sensitivity = 100f;
        private float cameraTargetPitch = 0f;
        private float rotationVelocity = 0f;

        private float topClamp = 45f;
        private float bottomClamp = -90f;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
        }

        private void LateUpdate()
        {
            CameraRotate();
        }
        #endregion

        #region Custom Method
        private void CameraRotate()
        {
            if (input.Look.sqrMagnitude < 0.01f) return;

            rotationVelocity = input.Look.x * rotationSpeed * Time.deltaTime * sensitivity;
            transform.Rotate(Vector3.up * rotationVelocity);
            cameraTargetPitch -= input.Look.y * rotationSpeed * sensitivity * Time.deltaTime;
            cameraRoot.localRotation = Quaternion.Euler(ClampAngle(cameraTargetPitch, topClamp, bottomClamp), 0f, 0f);

        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
        #endregion
    }
}