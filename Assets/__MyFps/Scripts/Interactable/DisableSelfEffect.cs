using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 아이템 획득 시 자기 자신(게임 오브젝트)을 비활성화하거나 파괴하는 효과 컴포넌트
    /// </summary>
    public class DisableSelfEffect : MonoBehaviour, IInteractEffect
    {
        public enum DisableMethod
        {
            SetActiveFalse,
            Destroy
        }
        
        [Tooltip("비활성화 방식 선택")]
        public DisableMethod disableMethod = DisableMethod.SetActiveFalse;

        public void ExecuteEffect(GameObject interactor)
        {
            if (disableMethod == DisableMethod.SetActiveFalse)
            {
                gameObject.SetActive(false);
            }
            else if (disableMethod == DisableMethod.Destroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
