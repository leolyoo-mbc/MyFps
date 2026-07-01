using UnityEngine;
using UnityEditor;

public class SetupRobotColliders
{
    public static void Execute()
    {
        GameObject robotRoot = GameObject.Find("=== Dynamic ===/Robot/ROOT/Hips");
        if (robotRoot == null)
        {
            Debug.LogError("Could not find '=== Dynamic ===/Robot/ROOT/Hips'");
            return;
        }

        Transform hips = robotRoot.transform;
        
        // Hips (Box)
        AddBox(hips, new Vector3(0.3f, 0.2f, 0.2f));

        // Spine
        Transform spine1 = hips.Find("Spine1");
        if (spine1) AddBox(spine1, new Vector3(0.3f, 0.2f, 0.2f));
        Transform spine2 = spine1?.Find("Spine2");
        if (spine2) AddBox(spine2, new Vector3(0.3f, 0.2f, 0.2f));

        // Neck/Head
        Transform neck = spine2?.Find("Neck");
        Transform head = neck?.Find("Head");
        if (head) {
            if (head.GetComponent<Collider>() == null) {
                SphereCollider sc = head.gameObject.AddComponent<SphereCollider>();
                sc.radius = 0.15f;
                sc.center = new Vector3(0, 0.1f, 0);
            }
        }

        // Left Arm
        Transform lShoulder = spine2?.Find("LeftShoulder");
        Transform lUpperArm = lShoulder?.Find("LeftUpperArm");
        Transform lArm = lUpperArm?.Find("LeftArm");
        Transform lHand = lArm?.Find("LeftHand");
        
        if (lUpperArm && lArm) AddCapsuleBetween(lUpperArm, lArm, 0.08f);
        if (lArm && lHand) AddCapsuleBetween(lArm, lHand, 0.06f);

        // Right Arm
        Transform rShoulder = spine2?.Find("RightShoulder");
        Transform rUpperArm = rShoulder?.Find("RightUpperArm");
        Transform rArm = rUpperArm?.Find("RightArm");
        Transform rHand = rArm?.Find("RightHand");

        if (rUpperArm && rArm) AddCapsuleBetween(rUpperArm, rArm, 0.08f);
        if (rArm && rHand) AddCapsuleBetween(rArm, rHand, 0.06f);

        // Left Leg
        Transform lLeg = hips.Find("LeftLeg");
        Transform lCalf = lLeg?.Find("LeftCalf");
        Transform lFoot = lCalf?.Find("LeftFoot");

        if (lLeg && lCalf) AddCapsuleBetween(lLeg, lCalf, 0.1f);
        if (lCalf && lFoot) AddCapsuleBetween(lCalf, lFoot, 0.09f);

        // Right Leg
        Transform rLeg = hips.Find("RightLeg");
        Transform rCalf = rLeg?.Find("RightCalf");
        Transform rFoot = rCalf?.Find("RightFoot");

        if (rLeg && rCalf) AddCapsuleBetween(rLeg, rCalf, 0.1f);
        if (rCalf && rFoot) AddCapsuleBetween(rCalf, rFoot, 0.09f);

        Debug.Log("Robot colliders setup successfully!");
    }

    static void AddBox(Transform t, Vector3 size)
    {
        if (t.GetComponent<Collider>() == null)
        {
            BoxCollider bc = t.gameObject.AddComponent<BoxCollider>();
            bc.size = size;
        }
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
