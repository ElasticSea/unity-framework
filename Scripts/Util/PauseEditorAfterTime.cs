#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class PauseEditorAfterTime : MonoBehaviour
    {
        [SerializeField] private float time;

        private bool paused;

        private void Update()
        {
            if (paused == false)
            {
                if (Time.time > time)
                {
#if UNITY_EDITOR
                    EditorApplication.isPaused = true;
#endif
                    paused = true;
                }
            }
        }
    }
}