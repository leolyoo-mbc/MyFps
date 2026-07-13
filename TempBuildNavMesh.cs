using UnityEngine;
using UnityEditor;

public class TempBuildNavMesh {
    public static void Execute() {
        GameObject root = new GameObject("NavMeshEnvironment");
        
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.SetParent(root.transform);
        ground.transform.localScale = new Vector3(2f, 1f, 2f);
        GameObjectUtility.SetStaticEditorFlags(ground, StaticEditorFlags.NavigationStatic);

        GameObject obs1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs1.name = "Obstacle1_Cube";
        obs1.transform.SetParent(root.transform);
        obs1.transform.position = new Vector3(3f, 1f, 3f);
        obs1.transform.localScale = new Vector3(2f, 2f, 2f);
        GameObjectUtility.SetStaticEditorFlags(obs1, StaticEditorFlags.NavigationStatic);

        GameObject obs2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obs2.name = "Obstacle2_Cylinder";
        obs2.transform.SetParent(root.transform);
        obs2.transform.position = new Vector3(-4f, 1f, 2f);
        obs2.transform.localScale = new Vector3(2f, 2f, 2f);
        GameObjectUtility.SetStaticEditorFlags(obs2, StaticEditorFlags.NavigationStatic);

        GameObject obs3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs3.name = "Obstacle3_Wall";
        obs3.transform.SetParent(root.transform);
        obs3.transform.position = new Vector3(2f, 0.5f, -5f);
        obs3.transform.localScale = new Vector3(4f, 1f, 1f);
        GameObjectUtility.SetStaticEditorFlags(obs3, StaticEditorFlags.NavigationStatic);

        GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ramp.name = "Ramp";
        ramp.transform.SetParent(root.transform);
        ramp.transform.position = new Vector3(-5f, 0.6f, -5f);
        ramp.transform.localScale = new Vector3(3f, 0.2f, 5f);
        ramp.transform.rotation = Quaternion.Euler(-15f, 0f, 0f);
        GameObjectUtility.SetStaticEditorFlags(ramp, StaticEditorFlags.NavigationStatic);

        Selection.activeGameObject = root;
        Undo.RegisterCreatedObjectUndo(root, "Create NavMesh Environment");
    }
}
