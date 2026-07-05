using System;
using UnityEngine;

namespace MyFps
{
    [CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Data/PlayerStatsData")]
    public class PlayerStatsData : ScriptableObject
    {
        [Tooltip("현재 보유 중인 총알 개수")]
        [SerializeField] private int ammoCount = 0;

        // 총알이 변경될 때 UI 등에서 들을 수 있는 이벤트
        public event Action<int> OnAmmoChanged;

        public int AmmoCount
        {
            get => ammoCount;
            set
            {
                if (ammoCount != value)
                {
                    ammoCount = value;
                    OnAmmoChanged?.Invoke(ammoCount);
                }
            }
        }

        public void ResetToDefault()
        {
            AmmoCount = 0; // 게임 시작 시 초기 지급량
        }
    }
}
