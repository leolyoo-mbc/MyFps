using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement; // 에디터 전용 씬 매니저 네임스페이스
#endif

namespace MySample
{
    public class AdditiveSceneLoader : MonoBehaviour
    {
        // 씬을 추가(Additive) 모드로 로드하는 함수
        public void LoadSubScene(string scenePath)
        {
#if UNITY_EDITOR
            // 1. 경로에서 확장자를 제외한 씬의 "이름"만 추출합니다. (예: SubScene_A)
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            // 이미 해당 씬이 로드되어 있는지 확인
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.Log($"[Editor Only] {sceneName} 씬을 경로를 통해 로드합니다.");

                // 에디터 모드 전용 씬 로드 메서드 (빌드 세팅 무시)
                EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Additive));
            }
            else
            {
                Debug.LogWarning($"{sceneName} 씬은 이미 로드되어 있습니다!");
            }
#else
            Debug.LogError("이 코드는 에디터 플레이 모드에서만 동작합니다!");
#endif
        }

        // 씬을 언로드(제거)하는 함수
        public void UnloadSubScene(string scenePath)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                Debug.Log($"{sceneName} 씬을 언로드합니다.");
                SceneManager.UnloadSceneAsync(sceneName);
            }
            else
            {
                Debug.LogWarning($"{sceneName} 씬은 로드되어 있지 않습니다!");
            }
        }
    }
}