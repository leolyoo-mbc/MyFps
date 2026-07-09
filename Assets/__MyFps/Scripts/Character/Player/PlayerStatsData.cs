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

        [Tooltip("열쇠 보유 여부")]
        [SerializeField] private bool key = false;

        public event Action<bool> OnKeyStatusChanged;

        public bool Key
        {
            get => key;
            set
            {
                if (key != value)
                {
                    key = value;
                    OnKeyStatusChanged?.Invoke(key);
                }
            }
        }

        [Tooltip("왼쪽 퍼즐 눈(Left Eye) 보유 여부")]
        [SerializeField] private bool puzzleLeftEye = false;

        public event Action<bool> OnPuzzleLeftEyeStatusChanged;

        public bool PuzzleLeftEye
        {
            get => puzzleLeftEye;
            set
            {
                if (puzzleLeftEye != value)
                {
                    puzzleLeftEye = value;
                    OnPuzzleLeftEyeStatusChanged?.Invoke(puzzleLeftEye);
                }
            }
        }

        [Tooltip("오른쪽 퍼즐 눈(Right Eye) 보유 여부")]
        [SerializeField] private bool puzzleRightEye = false;

        public event Action<bool> OnPuzzleRightEyeStatusChanged;

        public bool PuzzleRightEye
        {
            get => puzzleRightEye;
            set
            {
                if (puzzleRightEye != value)
                {
                    puzzleRightEye = value;
                    OnPuzzleRightEyeStatusChanged?.Invoke(puzzleRightEye);
                }
            }
        }

        public void ResetToDefault()
        {
            AmmoCount = 0; // 게임 시작 시 초기 지급량
            Key = false;
            PuzzleLeftEye = false;
            PuzzleRightEye = false;
        }
    }
}
