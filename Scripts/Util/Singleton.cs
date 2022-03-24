using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance)
                {
                    return instance;
                }

                instance = FindObjectOfType<T>();
                if (instance)
                {
                    return instance;
                }
                
                instance = new GameObject(typeof(Singleton<T>).GetSimpleAliasName()).AddComponent<T>();
                return instance;
            }
        }

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