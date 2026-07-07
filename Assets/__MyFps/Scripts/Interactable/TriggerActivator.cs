using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어와 충돌(Trigger)했을 때 조건(Condition)을 검사하고 효과(Effect)를 실행하는 발동기 컴포넌트
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerActivator : MonoBehaviour
    {
        public string itemName = "아이템";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // 1. 이 오브젝트에 붙은 모든 조건(Condition) 스크립트를 가져와 검사
                var conditions = GetComponents<IInteractCondition>();
                foreach (var condition in conditions)
                {
                    if (!condition.CanInteract(other.gameObject))
                    {
                        // 단 하나라도 실패하면 상호작용 중단
                        // (필요 시 "가방이 꽉 찼습니다" 등의 알림을 띄울 수 있음)
                        return;
                    }
                }

                print($"{itemName} 상호작용 성공 (Trigger)");

                // 2. 모든 조건을 통과했으므로 부착된 모든 효과(Effect) 스크립트를 실행
                var effects = GetComponents<IInteractEffect>();
                foreach (var effect in effects)
                {
                    effect.ExecuteEffect(other.gameObject);
                }
            }
        }
    }
}
