using UnityEngine;

namespace MyFps
{
    public interface IInteractable
    {
        public string ActionText { get; }

        public abstract void OnInteract(GameObject interactor);
    }
}
