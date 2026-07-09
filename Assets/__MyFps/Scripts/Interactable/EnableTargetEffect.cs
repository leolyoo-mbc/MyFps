using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 상호작용 성공 시 특정 대상(GameObject)을 활성화하는 이펙트 스크립트.
    /// (예: 퍼즐을 맞췄을 때 퍼즐 조각 그림을 화면에 표시)
    /// </summary>
    public class EnableTargetEffect : MonoBehaviour, IInteractEffect
    {
        [SerializeField, Tooltip("상호작용 시 활성화할 대상 오브젝트 (예: 그림/퍼즐 조각 모델)")]
        private GameObject targetObject;

        [SerializeField, Tooltip("게임 시작 시(Start) 대상 오브젝트를 자동으로 비활성화할지 여부")]
        private bool disableOnStart = true;

        private void Start()
        {
            if (disableOnStart && targetObject != null)
            {
                targetObject.SetActive(false);
            }
        }

        public void ExecuteEffect(GameObject interactor)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }
        }
    }
}
