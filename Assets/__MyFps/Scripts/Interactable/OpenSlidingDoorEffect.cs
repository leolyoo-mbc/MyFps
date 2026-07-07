using UnityEngine;

namespace MyFps
{
    public class OpenSlidingDoorEffect : MonoBehaviour, IInteractEffect, IDynamicActionText
    {
        [SerializeField] private SlidingDoorController doorController;

        public void ExecuteEffect(GameObject interactor)
        {
            if (doorController != null)
            {
                doorController.TargetOpenAmount = doorController.TargetOpenAmount > 0f ? 0f : 1f;
            }
        }

        public string GetActionText()
        {
            if (doorController != null)
            {
                return doorController.TargetOpenAmount > 0f ? "CLOSE THE DOOR" : "OPEN DOOR";
            }
            return "";
        }
    }
}
