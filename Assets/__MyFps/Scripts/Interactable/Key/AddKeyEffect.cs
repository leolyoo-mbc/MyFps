using UnityEngine;

namespace MyFps
{
    public class AddKeyEffect : MonoBehaviour, IInteractEffect
    {
        [SerializeField] private PlayerStatsData stats;

        public void ExecuteEffect(GameObject interactor)
        {
            if (stats != null)
            {
                stats.Key = true;
                Debug.Log("Key acquired!");
            }
        }
    }
}
