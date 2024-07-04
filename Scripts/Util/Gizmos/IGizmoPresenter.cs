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

            var gameGizmoProvider = new GameGizmoProvider(CreateMaterial());
            var editorGizmoProvider = new EditorGizmoProvider();
            
            void Draw(IGizmoProvider provider)
            {
                provider.Color = Color.white;
                provider.Matrix = Matrix4x4.identity;
                presenter.OnGameOrEditorGizmoDraw(provider);
            }
            
            camera = camera ?? Camera.main;
            if (camera)
            {
                camera.gameObject.GetOrAddComponent<OnPostRenderCallback>().OnPostRenderEvent += () => Draw(gameGizmoProvider);
            }

            target.gameObject.GetOrAddComponent<OnDrawGizmoCallback>().OnDrawGizmosEvent += () => Draw(editorGizmoProvider);
#endif
        }
        
        public static void SetupGame(IGizmoPresenter presenter, Camera camera = null)
        {
#if UNITY_EDITOR

            var gameGizmoProvider = new GameGizmoProvider(CreateMaterial());
            
            void Draw(IGizmoProvider provider)
            {
                provider.Color = Color.white;
                provider.Matrix = Matrix4x4.identity;
                presenter.OnGameOrEditorGizmoDraw(provider);
            }
            
            camera = camera ?? Camera.main;
            camera.gameObject.GetOrAddComponent<OnPostRenderCallback>().OnPostRenderEvent += () => Draw(gameGizmoProvider);
#endif
        }
        
        public static void DrawEditor(IGizmoPresenter presenter)
        {
#if UNITY_EDITOR

            var editorGizmoProvider = new EditorGizmoProvider();
            editorGizmoProvider.Color = Color.white;
            editorGizmoProvider.Matrix = Matrix4x4.identity;
            presenter.OnGameOrEditorGizmoDraw(editorGizmoProvider);
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
            lineMaterial.SetInt("_Cull", 0);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
            // Dont test the depth buffer
            lineMaterial.SetInt("_ZTest", 0);
            // Write at the end after everything
            lineMaterial.renderQueue = 5000;
            return lineMaterial;
        }
    }
}