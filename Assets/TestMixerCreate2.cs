using UnityEngine;
using UnityEditor;

public class TestMixerCreate2
{
    public static void Run()
    {
        var mixer = ScriptableObject.CreateInstance("UnityEditor.Audio.AudioMixerController");
        if (mixer != null)
        {
            Debug.Log("Mixer created via CreateInstance! Saving to Assets/TestMixer.mixer");
            AssetDatabase.CreateAsset(mixer, "Assets/TestMixer.mixer");
            AssetDatabase.SaveAssets();
            Debug.Log("Mixer saved successfully!");
        }
        else
        {
            Debug.Log("Failed to create mixer via CreateInstance.");
        }
    }
}
