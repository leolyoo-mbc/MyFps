using System;
using UnityEngine;

namespace MyFps
{
    [CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Data/PlayerStatsData")]
    public class PlayerStatsData : ScriptableObject
    {
        [SerializeField] private int[] items;
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth = 20f;
        
        public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
        public float MaxHealth { get => maxHealth; }

        public event Action<ItemType, int> OnItemChanged;

        public int this[ItemType type]
        {
            get
            {
                int index = (int)type;
                if (items != null && index >= 0 && index < items.Length)
                {
                    return items[index];
                }
                return 0;
            }
            set
            {
                int index = (int)type;
                if (items != null && index >= 0 && index < items.Length)
                {
                    items[index] = value;
                    OnItemChanged?.Invoke(type, value);
                }
            }
        }

        public void ResetToDefault()
        {
            int count = (int)ItemType.Count;

            // 배열이 없거나 크기가 다를 때만 1회 새로 할당 (GC 방지)
            if (items == null || items.Length != count)
            {
                items = new int[count];
            }

            for (int i = 0; i < count; i++)
            {
                items[i] = 0;
            }
            currentHealth = maxHealth;
        }

    }
}
