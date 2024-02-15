using System;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class GameGizmos
    {
        private static Material lineMaterial;

        private static void EnsureMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        private static void BeforeDraw()
        {
            EnsureMaterial();
        
            lineMaterial.SetPass(0);
            
            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(Gizmos.matrix);
            GL.Begin(GL.LINES);
            GL.Color(Gizmos.color);
        }

        private static void AfterDraw()
        {
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            try
            {
                Gizmos.DrawLine(from, to);
            }
            catch (Exception e)
            {
            }
            BeforeDraw();
            GL.Vertex3(from.x, from.y, from.z);
            GL.Vertex3(to.x, to.y, to.z);
            AfterDraw();
        }

        public static void DrawRay(Vector3 from, Vector3 direction)
        {
            try
            {
                Gizmos.DrawRay(from, direction);
            }
            catch (Exception e)
            {
            }
            BeforeDraw();
            GL.Vertex3(from.x, from.y, from.z);
            GL.Vertex3(from.x + direction.x, from.y + direction.y, from.z + direction.z);
            AfterDraw();
        }
    }
}