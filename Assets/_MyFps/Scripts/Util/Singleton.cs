using UnityEngine;

namespace MyFps
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        #region Variables
        protected static T _instance;
        private static bool applicationIsQuitting = false;
        #endregion

        #region Property
        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                if (_instance == null)
                {
                    // 1. 씬에 이미 배치되어 있는지 확인
                    _instance = FindFirstObjectByType<T>();

                    // 2. 씬에 없다면, 빈 오브젝트를 하나 만들어서 컴포넌트를 붙여줌 (자동 생성)
                    if (_instance == null)
                    {
                        GameObject go = new(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
            }
            else
            {
                // 이미 인스턴스가 존재한다면 중복된 것은 파괴
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
        #endregion
    }
}