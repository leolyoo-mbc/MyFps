using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MyFps;

public class RevertSceneFaderSetupScript
{
    [MenuItem("Tools/Revert Scene Fader")]
    public static void Execute()
    {
        string log = "";

        // 1. Find Canvas
        GameObject canvasObj = GameObject.Find("Canvas_Overlay");
        if (canvasObj != null) {
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            Undo.RecordObject(canvas, "Revert Canvas Render Mode");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = null;
            log += "Reverted Canvas_Overlay to ScreenSpaceOverlay.\n";
        } else {
            Debug.LogError("Could not find Canvas_Overlay");
            return;
        }

        // 2. Find SceneFader
        SceneFader fader = Object.FindObjectOfType<SceneFader>();
        if (fader != null) {
            Undo.RecordObject(fader.transform, "Revert Fader Position");
            Vector3 localPos = fader.transform.localPosition;
            localPos.z = 0f; // Reset to 0 (default position relative to camera)
            fader.transform.localPosition = localPos;
            log += $"Reverted Fader local Z position to {localPos.z}.\n";
        } else {
            log += "Warning: Could not find SceneFader component in the active scene.\n";
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log(log + "Revert Complete.");
    }
}
