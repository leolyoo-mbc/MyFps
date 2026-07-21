using System.IO;
using UnityEngine;

namespace MyFps
{
    [CreateAssetMenu(fileName = "SaveLoadSystem", menuName = "Systems/SaveLoadSystem")]
    public class SaveLoadSystem : ScriptableObject
    {
        [Header("Data References")]
        [SerializeField] private PlayerStatsData playerStats;

        [System.Serializable]
        private class SavePayload
        {
            public int savedSceneIndex;
            public string playerStatsJson;
        }

        private string SaveFilePath => Application.persistentDataPath + "/save.json";

        public bool HasSaveFile() => File.Exists(SaveFilePath);

        public void SaveData(int currentSceneIndex)
        {
            if (playerStats == null)
            {
                Debug.LogWarning("SaveLoadSystem: playerStats is not assigned.");
                return;
            }

            SavePayload payload = new SavePayload
            {
                savedSceneIndex = currentSceneIndex,
                playerStatsJson = JsonUtility.ToJson(playerStats)
            };

            string json = JsonUtility.ToJson(payload, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Game Saved to {SaveFilePath}");
        }

        public void CheckSaveAndSetButton(UnityEngine.UI.Button button)
        {
            button.interactable = HasSaveFile();
        }

        public int LoadSaveData()
        {
            if (!HasSaveFile()) return -1;

            if (playerStats == null)
            {
                Debug.LogWarning("SaveLoadSystem: playerStats is not assigned.");
                return -1;
            }

            string json = File.ReadAllText(SaveFilePath);
            SavePayload payload = JsonUtility.FromJson<SavePayload>(json);

            JsonUtility.FromJsonOverwrite(payload.playerStatsJson, playerStats);

            return payload.savedSceneIndex + 1;
        }

        public void LoadGameAndFade(SceneFader fader)
        {
            int sceneIndex = LoadSaveData();
            if (sceneIndex != -1 && fader != null)
            {
                fader.FadeTo(sceneIndex);
            }
        }
    }
}
