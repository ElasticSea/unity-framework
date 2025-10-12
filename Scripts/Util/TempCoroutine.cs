using System.Collections;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public static class TempCoroutine
    {
        private class Runner : MonoBehaviour { }

        public static void Run(IEnumerator routine)
        {
            var go = new GameObject("[TempCoroutine]");
            var runner = go.AddComponent<Runner>();
            runner.StartCoroutine(RunAndDestroy(routine, go));
        }

        private static IEnumerator RunAndDestroy(IEnumerator routine, GameObject go)
        {
            yield return routine;
            Object.Destroy(go);
        }
    }
}