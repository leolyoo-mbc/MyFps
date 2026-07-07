using UnityEngine;

namespace MyFps
{
    public class RequireKeyCondition : MonoBehaviour, IInteractCondition
    {
        [SerializeField] private PlayerStatsData stats;

        public bool CanInteract(GameObject interactor)
        {
            if (stats == null) return false;
            if (!stats.HasKey)
            {
                Debug.Log("You need the HeartKey!");
                return false;
            }
            return true;
        }
    }
}
