using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util.Callbacks;
using UnityEngine;

namespace ElasticSea.Framework.Util.Gizmo
{
    public interface IGizmoPresenter
    {
        void OnGameOrEditorGizmoDraw(IGizmoProvider provider);
        
        public static void Setup(IGizmoPresenter presenter, MonoBehaviour target, Camera camera = null)
        {
#if UNITY_EDITOR
            camera = camera ?? Camera.main;

            var gameGizmoProvider = new GameGizmoProvider(CreateMaterial());
            
            camera.gameObject.GetOrAddComponent<OnPostRenderCallback>().OnPostRenderEvent += () =>
            {
                presenter.OnGameOrEditorGizmoDraw(gameGizmoProvider);
            };

            var editorGizmoProvider = new EditorGizmoProvider();
            target.gameObject.GetOrAddComponent<OnDrawGizmoCallback>().OnDrawGizmosEvent += () =>
            {
                presenter.OnGameOrEditorGizmoDraw(editorGizmoProvider);
            };
#endif
        }

        private static Material CreateMaterial()
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            var lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            // lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            // lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
            return lineMaterial;
        }
    }
}