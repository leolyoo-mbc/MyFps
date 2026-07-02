using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // 씬 로드를 위해 필요

namespace MyFps
{
    public class PauseManager : MonoBehaviour
    {
        #region Variables
        private InputSystem_Actions inputActions;

        [Header("UI References")]
        public GameObject pauseUI; // 여기에 Canvas_Overlay/PauseUI 를 연결합니다.
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // CharacterInput과 별개로 UI 조작을 위한 인스턴스를 하나 더 만듭니다.
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            // Pause 기능이 포함된 UI 맵만 활성화합니다.
            inputActions.UI.Enable();
            // UI 맵의 Pause 액션에 이벤트(콜백)를 연결합니다.
            inputActions.UI.Pause.performed += OnPauseToggle;
        }

        private void OnDisable()
        {
            inputActions.UI.Disable();
            inputActions.UI.Pause.performed -= OnPauseToggle;
        }
        #endregion

        #region Custom Method
        // P키를 눌렀을 때 실행될 콜백 함수
        private void OnPauseToggle(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        public void TogglePause()
        {
            pauseUI.SetActive(!pauseUI.activeSelf);
            Time.timeScale = pauseUI.activeSelf ? 0f : 1f;
            Cursor.lockState = pauseUI.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !pauseUI.activeSelf;
        }

        // 버튼 컴포넌트에서 호출해야 하므로 public으로 만듭니다.
        public void LoadMainMenu()
        {
            Time.timeScale = 1f; // 씬 넘어가기 전 시간 복구 필수!
            SceneManager.LoadScene("MainMenu");
        }
        #endregion
    }
}