using UnityEngine;

namespace MyFps
{
    public class ModifyItemEffect : MonoBehaviour, IInteractEffect
    {
        [SerializeField] private ItemType itemType;
        [SerializeField, Tooltip("지급/회수할 수량 (음수면 회수)")] 
        private int amount = 1;

        public void ExecuteEffect(GameObject interactor)
        {
            var inventory = interactor.GetComponentInParent<IInventory>();
            if (inventory != null)
            {
                inventory.ModifyItem(itemType, amount);
                
                if (amount > 0)
                {
                    Debug.Log($"Acquired {amount} of {itemType}!");
                }
                else
                {
                    Debug.Log($"Used {-amount} of {itemType}.");
                }
            }
        }
    }
}
