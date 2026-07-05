using UnityEngine;

namespace MyFps
{
    public class PlayerLook : MonoBehaviour
    {
        #region Variables
        [Header("Input Intent")]
        public Vector2 lookIntent;

        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 100f; // 마우스 감도

        [Header("References")]
        [SerializeField] private Transform playerBody;   // 좌우 회전용 (보통 자신)
        [SerializeField] private Transform playerCamera; // 상하 회전용 (Main Camera)

        private float xRotation = 0f; // 카메라 상하 각도 누적용
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            lookIntent = Vector2.zero;

            // 활성화 시 마우스 커서를 화면 중앙에 고정하고 숨김
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnDisable()
        {
            // 비활성화 시 마우스 커서 복구
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (lookIntent == Vector2.zero) return;

            // 1. 입력 의지(마우스 이동량)를 실제 회전값으로 변환
            float mouseX = lookIntent.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookIntent.y * mouseSensitivity * Time.deltaTime;

            // 2. 카메라 상하(Pitch) 회전 누적 및 제한
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -89f, 89f); // 목이 꺾이지 않게 -89 ~ 89도로 제한

            // 3. 실제 회전 적용
            if (playerCamera != null)
            {
                // 카메라는 로컬 좌표계 기준으로 위아래(X축)로만 끄덕거림
                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }

            if (playerBody != null)
            {
                // 몸통은 Y축 기준으로 좌우로 뱅글뱅글 돎
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }
        #endregion
    }
}