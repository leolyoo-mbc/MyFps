using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 상호작용이 가능한 상태인지 체크하는 조건 인터페이스 (합성용 컴포넌트)
    /// </summary>
    public interface IInteractCondition
    {
        /// <summary>
        /// 상호작용 할 수 있는지 여부를 반환합니다.
        /// </summary>
        /// <param name="interactor">상호작용을 시도한 주체 (보통 플레이어)</param>
        /// <returns>가능하면 true, 불가능하면 false</returns>
        bool CanInteract(GameObject interactor);
    }
}
