using UnityEngine;

namespace MyFps
{
    public class PlayerInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private PlayerStatsData data;

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