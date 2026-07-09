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
                doorController.Open = !doorController.Open;
            }
        }

        public string GetActionText()
        {
            if (doorController != null)
            {
                return doorController.Open ? "CLOSE THE DOOR" : "OPEN DOOR";
            }
            return "";
        }
    }
}
