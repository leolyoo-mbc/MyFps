using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFps
{
    public class AutoSaveTrigger : MonoBehaviour
    {
        [SerializeField] private SaveLoadSystem saveSystem;

        private void Start()
        {
            if (saveSystem != null)
            {
                saveSystem.SaveData(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.LogWarning("AutoSaveTrigger: SaveLoadSystem is not assigned.");
            }
        }
    }
}
