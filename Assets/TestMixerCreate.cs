using UnityEngine;
using UnityEditor;
using System.Reflection;

public class TestMixerCreate
{
    public static void Run()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var controllerType = assembly.GetType("UnityEditor.Audio.AudioMixerController");
        
        if (controllerType != null)
        {
            Debug.Log("Found AudioMixerController type!");
            MethodInfo createMixerMethod = controllerType.GetMethod("CreateMixer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (createMixerMethod != null)
            {
                Debug.Log("Found CreateMixer method!");
            }
        }
    }
}
