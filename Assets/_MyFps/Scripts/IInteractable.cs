namespace MyFps
{
    public interface IInteractable
    {
        public abstract void OnFocus();
        public abstract void OnLostFocus();
        public abstract void OnInteract();
    }
}
