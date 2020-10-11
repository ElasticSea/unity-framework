using UnityEngine;

namespace Core.Util
{
    // Just like regular singleton, but we don't call DontDestroyOnLoad and free the instance on OnDestroy
    public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
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
                OnSingletonAwake();
                return true;
            }

            DestroyImmediate(gameObject);
            return false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                instance = null;
            }
        }

        protected abstract void OnSingletonAwake();
    }
}