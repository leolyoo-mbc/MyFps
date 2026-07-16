using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace MySample
{
    public class AdditiveSceneLoader : MonoBehaviour
    {
        [Header("Airlock Doors")]
        [Tooltip("지나온 씬(A) 방향의 문")]
        public GameObject doorA; 
        [Tooltip("다음 씬(B) 방향의 문")]
        public GameObject doorB; 

        [Header("Scene Paths")]
        public string scenePathA = "Assets/_Sample/12AdditiveSceneLoadingTest/SubScene_A.unity";
        public string scenePathB = "Assets/_Sample/12AdditiveSceneLoadingTest/SubScene_B.unity";

        private bool isTransitioning = false;

        // 씬을 추가(Additive) 모드로 로드하는 함수
        public void LoadSubScene(string scenePath)
        {
#if UNITY_EDITOR
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.Log($"[Editor Only] {sceneName} 씬을 경로를 통해 로드합니다.");
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

        // ==========================================
        // [Airlock 시스템 추가 로직]
        // ==========================================

        private void OnTriggerEnter(Collider other)
        {
            // 태그가 Player인 오브젝트가 트리거 안에 들어왔고, 로딩 중이 아닐 때
            if (other.CompareTag("Player") && !isTransitioning)
            {
                StartCoroutine(AirlockTransitionRoutine());
            }
        }

        private IEnumerator AirlockTransitionRoutine()
        {
            isTransitioning = true;
            
            string nameA = System.IO.Path.GetFileNameWithoutExtension(scenePathA);
            string nameB = System.IO.Path.GetFileNameWithoutExtension(scenePathB);

            // B가 로드되어 있다면 B에서 A로 돌아가는 중. 그 외에는 A에서 B로 가는 중이라고 판단
            bool isBLoaded = SceneManager.GetSceneByName(nameB).isLoaded;

            string unloadPath = isBLoaded ? scenePathB : scenePathA;
            string loadPath = isBLoaded ? scenePathA : scenePathB;
            
            GameObject doorToClose = isBLoaded ? doorB : doorA;
            GameObject doorToOpen = isBLoaded ? doorA : doorB;

            string unloadName = System.IO.Path.GetFileNameWithoutExtension(unloadPath);
            string loadName = System.IO.Path.GetFileNameWithoutExtension(loadPath);

            Debug.Log($"<color=cyan>[Airlock] 플레이어 진입. 등 뒤의 문을 닫습니다.</color>");
            if (doorToClose != null) doorToClose.SetActive(true);

            // 0.5초 대기
            yield return new WaitForSeconds(0.5f);

#if UNITY_EDITOR
            // 2. 이전 씬 언로드
            if (SceneManager.GetSceneByName(unloadName).isLoaded)
            {
                Debug.Log($"<color=orange>[Airlock] 지나온 씬({unloadName}) 언로드 중...</color>");
                SceneManager.UnloadSceneAsync(unloadName);
            }

            // 3. 다음 씬 비동기 로딩 시작
            if (!SceneManager.GetSceneByName(loadName).isLoaded)
            {
                Debug.Log($"<color=yellow>[Airlock] 다음 씬({loadName}) 비동기 로딩 시작!</color>");
                AsyncOperation op = EditorSceneManager.LoadSceneAsyncInPlayMode(loadPath, new LoadSceneParameters(LoadSceneMode.Additive));
                
                while (!op.isDone)
                {
                    yield return null;
                }
                Debug.Log($"<color=green>[Airlock] 다음 씬({loadName}) 로딩 완료!</color>");
            }
#else
            Debug.LogError("이 코드는 에디터 플레이 모드에서만 동작합니다!");
#endif

            // 4. 로딩 완료 시 앞쪽 문 열기
            Debug.Log("<color=cyan>[Airlock] 앞쪽 문을 엽니다. 나가셔도 좋습니다.</color>");
            if (doorToOpen != null) doorToOpen.SetActive(false);
            
            // 플레이어가 문 밖으로 완전히 빠져나갈 여유 시간을 줌
            yield return new WaitForSeconds(1.5f);
            
            // 다시 반대쪽으로 이동할 수 있도록 상태 초기화
            isTransitioning = false;
        }
    }
}