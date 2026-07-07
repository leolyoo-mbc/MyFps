using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// (예시용) 플레이어의 탄약이 가득 찼는지 검사하는 조건 컴포넌트
    /// </summary>
    public class CheckAmmoMaxCondition : MonoBehaviour, IPickupCondition
    {
        [Tooltip("플레이어 스탯 데이터 (이후 적절히 연결)")]
        // public PlayerStatsData playerStats;

        public bool CanPickup(GameObject interactor)
        {
            // 임시 구현: 탄약이 가득 찼다면 false를 반환하여 획득 중단
            // if (playerStats != null && playerStats.AmmoCount >= playerStats.MaxAmmo) return false;
            
            return true; 
        }
    }
}
