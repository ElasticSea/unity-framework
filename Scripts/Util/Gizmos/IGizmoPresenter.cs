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
            var editorGizmoProvider = new EditorGizmoProvider();
            
            void Draw(IGizmoProvider provider)
            {
                provider.Color = Color.white;
                provider.Matrix = Matrix4x4.identity;
                presenter.OnGameOrEditorGizmoDraw(provider);
            }
            
            camera.gameObject.GetOrAddComponent<OnPostRenderCallback>().OnPostRenderEvent += () => Draw(gameGizmoProvider);
            target.gameObject.GetOrAddComponent<OnDrawGizmoCallback>().OnDrawGizmosEvent += () => Draw(editorGizmoProvider);
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