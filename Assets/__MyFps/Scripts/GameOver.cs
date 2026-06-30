using UnityEngine;

namespace MyFps
{
    public class GameOver : MonoBehaviour
    {
        #region Variables
        [SerializeField] private SceneFader fader;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 마우스 잠금 해제
            Cursor.lockState = CursorLockMode.None;
            // 마우스 커서 보이기
            Cursor.visible = true;
        }
        #endregion

        #region Custom Method
        public void OnClickRestartButton(string sceneName)
        {
            fader.FadeTo(sceneName);
        }

        public void OnClickMenuButton()
        {
            Debug.Log("Goto Menu!!!");
        }
        #endregion
    }
}