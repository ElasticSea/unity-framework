using UnityEngine;

namespace Core.Util
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance => instance ?? (instance = FindObjectOfType<T>());

        protected bool Awake()
        {
            if (Instance == null)
            {
                instance = (T)this;
            }

            if (Instance == this)
            {
                if (Application.isPlaying) DontDestroyOnLoad(gameObject);
                OnSingletonAwake();
                return true;
            }

            DestroyImmediate(gameObject);
            return false;
        }

        protected abstract void OnSingletonAwake();
    }
}