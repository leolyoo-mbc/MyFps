using UnityEngine;

namespace MyFps
{
    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private PlayerStatsData data;

        private void Start()
        {
            // 게임 시작 시 초기화
            if (data != null) data.ResetToDefault();
        }

        public void ModifyItem(ItemType type, int amount)
        {
            data[type] += amount;
        }

        public bool HasItem(ItemType type, int amount = 1)
        {
            return data[type] >= amount;
        }
    }
}