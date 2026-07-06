using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MyFps;

public class SceneFaderSetupScript
{
    [MenuItem("Tools/Setup Scene Fader")]
    public static void Execute()
    {
        string log = "";

        // 1. Find Canvas
        GameObject canvasObj = GameObject.Find("Canvas_Overlay");
        if (canvasObj == null) {
            Debug.LogError("Could not find Canvas_Overlay");
            return;
        }
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        
        // 2. Find Main Camera
        Camera mainCam = Camera.main;
        if (mainCam == null) {
            Debug.LogError("Could not find Main Camera");
            return;
        }

        // 3. Update Canvas
        Undo.RecordObject(canvas, "Change Canvas Render Mode");
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = mainCam;
        canvas.planeDistance = 100f;
        log += "Set Canvas_Overlay to ScreenSpaceCamera with Main Camera.\n";

        // 4. Find SceneFader
        SceneFader fader = Object.FindObjectOfType<SceneFader>();
        if (fader != null) {
            Undo.RecordObject(fader.transform, "Change Fader Position");
            if (fader.transform.parent == mainCam.transform) {
                Vector3 localPos = fader.transform.localPosition;
                localPos.z = 10f; // Closer than 100f
                fader.transform.localPosition = localPos;
                log += $"Updated Fader local Z position to {localPos.z}.\n";
            } else {
                log += "Warning: SceneFader is not a child of Main Camera. Position not automatically updated.\n";
            }
            
            Renderer renderer = fader.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null) {
                log += $"SceneFader Material: {renderer.sharedMaterial.name}, Queue: {renderer.sharedMaterial.renderQueue}\n";
            }
        } else {
            log += "Warning: Could not find SceneFader component in the active scene.\n";
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log(log + "Setup Complete.");
    }
}
