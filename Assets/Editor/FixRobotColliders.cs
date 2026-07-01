using UnityEngine;
using UnityEditor;

public class FixRobotColliders
{
    public static void Execute()
    {
        GameObject robotRoot = GameObject.Find("=== Dynamic ===/Robot/ROOT/Hips");
        if (robotRoot == null) return;

        Transform hips = robotRoot.transform;
        Transform spine1 = hips.Find("Spine1");
        Transform spine2 = spine1?.Find("Spine2");
        Transform neck = spine2?.Find("Neck");
        Transform head = neck?.Find("Head");

        // Fix Head
        if (head != null)
        {
            SphereCollider oldSc = head.GetComponent<SphereCollider>();
            if (oldSc != null) GameObject.DestroyImmediate(oldSc);

            SphereCollider sc = head.gameObject.AddComponent<SphereCollider>();
            sc.radius = 0.15f;
            // Assuming bones extend in -X direction based on arm and spine hierarchy
            sc.center = new Vector3(-0.08f, 0, 0); 
        }

        // Fix Chest (Remove old BoxColliders)
        if (spine1 != null) {
            BoxCollider bc = spine1.GetComponent<BoxCollider>();
            if (bc != null) GameObject.DestroyImmediate(bc);
        }
        if (spine2 != null) {
            BoxCollider bc = spine2.GetComponent<BoxCollider>();
            if (bc != null) GameObject.DestroyImmediate(bc);
        }

        // Add Capsules for Chest
        if (spine1 && spine2) AddCapsuleBetween(spine1, spine2, 0.18f);
        if (spine2 && neck) AddCapsuleBetween(spine2, neck, 0.16f);

        // Shoulders
        Transform lShoulder = spine2?.Find("LeftShoulder");
        Transform lUpperArm = lShoulder?.Find("LeftUpperArm");
        if (lShoulder && lUpperArm) AddCapsuleBetween(lShoulder, lUpperArm, 0.08f);

        Transform rShoulder = spine2?.Find("RightShoulder");
        Transform rUpperArm = rShoulder?.Find("RightUpperArm");
        if (rShoulder && rUpperArm) AddCapsuleBetween(rShoulder, rUpperArm, 0.08f);

        Debug.Log("Robot colliders fixed successfully!");
    }

    static void AddCapsuleBetween(Transform start, Transform end, float radius)
    {
        if (start.GetComponent<Collider>() != null) return;

        CapsuleCollider cc = start.gameObject.AddComponent<CapsuleCollider>();
        cc.radius = radius;

        Vector3 localEnd = start.InverseTransformPoint(end.position);
        float length = localEnd.magnitude;
        cc.height = length;
        
        // Find main axis
        Vector3 absEnd = new Vector3(Mathf.Abs(localEnd.x), Mathf.Abs(localEnd.y), Mathf.Abs(localEnd.z));
        if (absEnd.x > absEnd.y && absEnd.x > absEnd.z) {
            cc.direction = 0; // X
            cc.center = new Vector3(localEnd.x * 0.5f, 0, 0);
        } else if (absEnd.y > absEnd.x && absEnd.y > absEnd.z) {
            cc.direction = 1; // Y
            cc.center = new Vector3(0, localEnd.y * 0.5f, 0);
        } else {
            cc.direction = 2; // Z
            cc.center = new Vector3(0, 0, localEnd.z * 0.5f);
        }
    }
}
