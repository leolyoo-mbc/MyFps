using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어가 상호작용(E키)했을 때 조건(Condition)을 검사하고 효과(Effect)를 실행하는 발동기 컴포넌트
    /// </summary>
    public class InteractActivator : MonoBehaviour, IInteractable
    {
        [SerializeField] private string actionText = "INTERACT";

        string IInteractable.ActionText
        {
            get
            {
                var dynamicText = GetComponent<IDynamicActionText>();
                if (dynamicText != null)
                {
                    return dynamicText.GetActionText();
                }
                return actionText;
            }
        }

        public void OnInteract(GameObject interactor)
        {
            // 1. 모든 조건 검사
            var conditions = GetComponents<IInteractCondition>();
            foreach (var condition in conditions)
            {
                if (!condition.CanInteract(interactor))
                {
                    print("조건 미달 (상호작용 실패)");
                    return; // 상호작용 중단
                }
            }

            // 2. 모든 효과 실행
            var effects = GetComponents<IInteractEffect>();
            foreach (var effect in effects)
            {
                effect.ExecuteEffect(interactor);
            }
        }
    }
}
