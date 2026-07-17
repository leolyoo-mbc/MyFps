using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace MySample
{
    // 가장 직관적이고 뼈대만 남긴 레이어드 씬(Layered Scene) 매니저입니다.
    public class LayeredSceneTester : MonoBehaviour
    {
        [Header("Scene Paths")]
        public string envScenePath = "Assets/_Sample/12AdditiveSceneLoadingTest/SubScene_A.unity";     // 무거운 배경 (지웠다 켰다 할 씬)
        public string dynamicScenePath = "Assets/_Sample/12AdditiveSceneLoadingTest/SubScene_B.unity"; // 상태 유지할 몬스터 (껐다 켰다 할 씬)

        private bool isSectorLoaded = false;

        // UI 캔버스 없이 화면 좌측 상단에 테스트 버튼을 띄웁니다.
        private void OnGUI()
        {
            if (!isSectorLoaded)
            {
                if (GUI.Button(new Rect(10, 10, 300, 50), "1. 섹터 진입 (Env 로드 & Dynamic 켜기)"))
                {
                    StartCoroutine(EnterSectorRoutine());
                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 10, 300, 50), "2. 섹터 퇴장 (Env 삭제 & Dynamic 숨기기)"))
                {
                    StartCoroutine(LeaveSectorRoutine());
                }
            }
        }

        private IEnumerator EnterSectorRoutine()
        {
            Debug.Log("<color=black><b>[1] 화면 암전 시작 (가상)</b></color>");
            yield return new WaitForSeconds(0.5f); // 페이드 아웃 연출 대기

#if UNITY_EDITOR
            // 1. Env 씬(배경)을 Load 합니다.
            string envName = System.IO.Path.GetFileNameWithoutExtension(envScenePath);
            if (!SceneManager.GetSceneByName(envName).isLoaded)
            {
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode(envScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                Debug.Log($"<color=green>[2] 무거운 배경 씬({envName}) 로드 완료</color>");
            }

            // 2. Dynamic 씬(몬스터) 처리
            string dynName = System.IO.Path.GetFileNameWithoutExtension(dynamicScenePath);
            if (!SceneManager.GetSceneByName(dynName).isLoaded)
            {
                // 게임 시작 후 최초 방문이면 새로 Load 합니다.
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode(dynamicScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                Debug.Log($"<color=yellow>[3] 몬스터 씬({dynName}) 최초 로드 완료</color>");
            }
            else
            {
                // 이미 로드된 적이 있다면(메모리에 있다면), 루트 오브젝트들을 다시 켭니다. (상태 완벽 유지!)
                Scene dynScene = SceneManager.GetSceneByName(dynName);
                foreach (GameObject rootObj in dynScene.GetRootGameObjects())
                {
                    rootObj.SetActive(true);
                }
                Debug.Log($"<color=yellow>[3] 몬스터 씬({dynName}) 다시 켜기 (이전 상태 그대로 부활!)</color>");
            }
#endif
            Debug.Log("<color=black><b>[4] 화면 페이드 인, 조작 시작</b></color>");
            isSectorLoaded = true;
        }

        private IEnumerator LeaveSectorRoutine()
        {
            Debug.Log("<color=black><b>[1] 화면 암전 시작 (가상)</b></color>");
            yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR
            // 1. Dynamic 씬(몬스터) 처리 - 절대로 Unload 하지 않습니다!
            string dynName = System.IO.Path.GetFileNameWithoutExtension(dynamicScenePath);
            Scene dynScene = SceneManager.GetSceneByName(dynName);
            if (dynScene.isLoaded)
            {
                // 씬에 있는 모든 최상위 오브젝트를 찾아서 비활성화(SetActive(false)) 합니다.
                foreach (GameObject rootObj in dynScene.GetRootGameObjects())
                {
                    rootObj.SetActive(false); // 물리, Update, 렌더링 모두 일시정지(동면)
                }
                Debug.Log($"<color=yellow>[2] 몬스터 씬({dynName}) 숨김 처리 (메모리에는 상태 보존됨)</color>");
            }

            // 2. Env 씬(배경) 처리 - 무거우니까 확실하게 메모리에서 날려버립니다.
            string envName = System.IO.Path.GetFileNameWithoutExtension(envScenePath);
            if (SceneManager.GetSceneByName(envName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(envName);
                Debug.Log($"<color=red>[3] 무거운 배경 씬({envName}) 완벽히 삭제(Unload)</color>");
            }
#endif
            Debug.Log("<color=black><b>[4] 다른 구역으로 이동 완료 (가상)</b></color>");
            isSectorLoaded = false;
        }
    }
}
