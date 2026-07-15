namespace MyFps
{
    public interface IDamageable
    {
        public abstract void TakeDamage(float damage, UnityEngine.Vector3 hitDirection = default);
    }
}
