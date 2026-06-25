using UnityEngine;

namespace MyFps
{
    public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
    {
        #region Unity Event Method
        protected override void Awake()
        {
            base.Awake();
            
            // 진짜 인스턴스일 때만 DontDestroyOnLoad 적용
            if (_instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion
    }
}