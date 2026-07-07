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
                stats.HasKey = true;
                Debug.Log("Key acquired!");
            }
        }
    }
}
