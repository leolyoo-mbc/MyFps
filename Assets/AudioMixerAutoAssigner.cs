using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using UnityEditor.SceneManagement;

public class AudioMixerAutoAssigner
{
    public static void Run()
    {
        // 1. Find the AudioMixer
        string[] mixerGuids = AssetDatabase.FindAssets("t:AudioMixer");
        AudioMixer targetMixer = null;
        
        foreach (var guid in mixerGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("MyFpsAudioMixer"))
            {
                targetMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(path);
                break;
            }
        }

        if (targetMixer == null)
        {
            Debug.LogError("MyFpsAudioMixer를 찾을 수 없습니다. 이름이 정확한지 확인해주세요.");
            return;
        }

        // 2. Find the SFX Group
        AudioMixerGroup[] sfxGroups = targetMixer.FindMatchingGroups("SFX");
        if (sfxGroups == null || sfxGroups.Length == 0)
        {
            Debug.LogError("MyFpsAudioMixer에 SFX라는 이름의 그룹이 없습니다.");
            return;
        }
        
        AudioMixerGroup sfxGroup = sfxGroups[0];
        int updatedCount = 0;

        // 3. Update Prefabs
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Skip Unity packages and standard assets if needed, but here we process all in Assets/__MyFps and Objects
            if (!path.StartsWith("Assets/__MyFps") && !path.StartsWith("Assets/Objects")) continue;

            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                var prefabRoot = editingScope.prefabContentsRoot;
                var audioSources = prefabRoot.GetComponentsInChildren<AudioSource>(true);
                bool modified = false;

                foreach (var source in audioSources)
                {
                    if (source.outputAudioMixerGroup != sfxGroup)
                    {
                        source.outputAudioMixerGroup = sfxGroup;
                        modified = true;
                        updatedCount++;
                    }
                }

                if (!modified)
                {
                    // No changes, we can just return and the scope won't save if we don't force it... 
                    // Actually, EditPrefabContentsScope always saves if we don't abort, but we want to avoid unnecessary saving if possible.
                }
            }
        }

        // 4. Update Current Open Scene(s)
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            if (scene.isLoaded)
            {
                var rootObjects = scene.GetRootGameObjects();
                foreach (var root in rootObjects)
                {
                    var audioSources = root.GetComponentsInChildren<AudioSource>(true);
                    foreach (var source in audioSources)
                    {
                        if (source.outputAudioMixerGroup != sfxGroup)
                        {
                            source.outputAudioMixerGroup = sfxGroup;
                            EditorUtility.SetDirty(source);
                            updatedCount++;
                        }
                    }
                }
            }
        }

        Debug.Log($"[성공] 총 {updatedCount}개의 AudioSource에 SFX 그룹을 할당 완료했습니다!");
    }
}
