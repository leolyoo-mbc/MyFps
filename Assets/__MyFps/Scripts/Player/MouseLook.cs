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
        private float cameraTargetYaw = 0f;

        [SerializeField] private float topClamp = 89f;
        [SerializeField] private float bottomClamp = -89f;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraTargetYaw = transform.eulerAngles.y;
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

            // 좌우 회전 (Yaw) - Transform.Rotate를 쓰면 미세한 오차가 누적되어 몸체가 기울어질 수 있으므로 절대값으로 제어합니다.
            cameraTargetYaw += input.Look.x * rotationSpeed * Time.deltaTime * sensitivity;
            transform.rotation = Quaternion.Euler(0f, cameraTargetYaw, 0f);

            // 상하 회전 (Pitch)
            cameraTargetPitch -= input.Look.y * rotationSpeed * sensitivity * Time.deltaTime;
            cameraTargetPitch = ClampAngle(cameraTargetPitch, bottomClamp, topClamp);

            cameraRoot.localRotation = Quaternion.Euler(cameraTargetPitch, 0f, 0f);
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