using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 조건이 충족되었을 때 실행될 아이템의 효과 인터페이스 (합성용 컴포넌트)
    /// </summary>
    public interface IPickupEffect
    {
        /// <summary>
        /// 아이템의 고유 효과를 실행합니다.
        /// </summary>
        /// <param name="interactor">상호작용을 시도한 주체 (보통 플레이어)</param>
        void ExecuteEffect(GameObject interactor);
    }
}
