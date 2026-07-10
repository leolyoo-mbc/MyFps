using UnityEngine;

namespace MyFps
{
    public class RequireItemCondition : MonoBehaviour, IInteractCondition
    {
        [SerializeField] private ItemType requiredItem;

        public bool CanInteract(GameObject interactor)
        {
            var inventory = interactor.GetComponentInParent<IInventory>();
            if (inventory == null) return false;

            if (!inventory.HasItem(requiredItem))
            {
                Debug.Log($"You need the {requiredItem}!");
                return false;
            }
            return true;
        }
    }
}
