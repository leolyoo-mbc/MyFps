using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class TempLightAnalyzer {
    public static void Run() {
        var sb = new StringBuilder();
        var lights = Object.FindObjectsOfType<Light>();
        int shadowCastingLights = 0;
        
        sb.AppendLine($"Total Active Lights in Scene: {lights.Length}");
        sb.AppendLine("--------------------------------------------------");
        
        foreach (var light in lights) {
            if (light.shadows != LightShadows.None) {
                shadowCastingLights++;
                sb.AppendLine($"- Name: {light.gameObject.name}");
                sb.AppendLine($"  Type: {light.type}");
                sb.AppendLine($"  Shadows: {light.shadows}");
                sb.AppendLine($"  Shadow Resolution: {light.shadowResolution}");
                sb.AppendLine("");
            }
        }
        
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine($"Total Lights casting shadows: {shadowCastingLights}");
        
        Debug.Log("[LightAnalyzerOutput]\n" + sb.ToString());
    }
}
