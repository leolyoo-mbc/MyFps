using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 지정된 모든 게임 오브젝트들이 활성화(Active) 상태일 때만 상호작용을 허용하는 조건 스크립트.
    /// (예: 퍼즐 조각 그림 2개가 모두 월드에 활성화되어 있어야만 문 열림 버튼 작동)
    /// </summary>
    public class RequireActiveObjectsCondition : MonoBehaviour, IInteractCondition
    {
        [SerializeField, Tooltip("이 오브젝트들이 모두 활성화되어 있어야 상호작용 가능")]
        private GameObject[] requiredActiveObjects;

        public bool CanInteract(GameObject interactor)
        {
            if (requiredActiveObjects == null || requiredActiveObjects.Length == 0) return true;

            foreach (var obj in requiredActiveObjects)
            {
                // 하나라도 비활성화 상태라면 조건 불만족
                if (obj == null || !obj.activeInHierarchy)
                {
                    Debug.Log("모든 퍼즐 조각이 월드에 배치되지 않았습니다.");
                    return false;
                }
            }
            return true;
        }
    }
}
