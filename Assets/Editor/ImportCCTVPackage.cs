using UnityEditor;
using UnityEngine;

public class ImportCCTVPackage
{
    public static void Execute()
    {
        string packagePath = "y:/[Project]MyFps/리소스/MainMenu/Camera_CCTV.unitypackage";
        AssetDatabase.ImportPackage(packagePath, false);
        Debug.Log("Imported Camera_CCTV.unitypackage");
    }
}
