using System;
using ElasticSea.Framework.Util.Gizmo;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class GameGizmoProvider : IGizmoProvider
    {
        private Material material;
        private Mesh sphereMesh;
        
        public Matrix4x4 Matrix
        {
            set => Gizmos.matrix = value;
        }

        public Color Color
        {
            set => Gizmos.color = value;
        }

        public GameGizmoProvider(Material material, Mesh sphereMesh)
        {
            this.material = material;
            this.sphereMesh = sphereMesh;
        }

        private void BeforeDraw()
        {
            material.SetPass(0);
            
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

        public void DrawLine(Vector3 from, Vector3 to)
        {
            BeforeDraw();
            DrawLineInternal(from, to);
            AfterDraw();
        }

        private void DrawLineInternal(Vector3 from, Vector3 to)
        {
            GL.Vertex3(from.x, from.y, from.z);
            GL.Vertex3(to.x, to.y, to.z);
        }

        public void DrawRay(Vector3 from, Vector3 direction)
        {
            BeforeDraw();
            GL.Vertex3(from.x, from.y, from.z);
            GL.Vertex3(from.x + direction.x, from.y + direction.y, from.z + direction.z);
            AfterDraw();
        }

        public void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
        {
            BeforeDraw();
            DrawCapsuleInternal(p1, p2, radius);
            AfterDraw();
        }

        public void DrawSphere(Vector3 p1, float radius)
        {
            material.SetPass(0);
            var matrix = Gizmos.matrix * Matrix4x4.TRS(p1, Quaternion.identity, Vector3.one * radius);
            Graphics.DrawMeshNow(sphereMesh, matrix, 0);
        }

        public void DrawWireSphere(Vector3 p1, float radius)
        {
            BeforeDraw();
            DrawWireSphereInternal(p1, radius);
            AfterDraw();
        }

        public void DrawWireCube(Vector3 center, Vector3 size)
        {
            BeforeDraw();
            DrawWireCubeInternal(center, size);
            AfterDraw();
        }

        public void DrawWireCubeInternal(Vector3 center, Vector3 size)
        {
            var p0 = center + new Vector3(size.x / 2, size.y / 2, size.z / 2);
            var p1 = center + new Vector3(size.x / 2, size.y / 2, -size.z / 2);
            var p2 = center + new Vector3(-size.x / 2, size.y / 2, size.z / 2);
            var p3 = center + new Vector3(-size.x / 2, size.y / 2, -size.z / 2);
            var p4 = center + new Vector3(size.x / 2, -size.y / 2, size.z / 2);
            var p5 = center + new Vector3(size.x / 2, -size.y / 2, -size.z / 2);
            var p6 = center + new Vector3(-size.x / 2, -size.y / 2, size.z / 2);
            var p7 = center + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2);
            
            DrawLineInternal(p0, p1);
            DrawLineInternal(p1, p2);
            DrawLineInternal(p2, p3);
            DrawLineInternal(p3, p0);
            
            DrawLineInternal(p4, p5);
            DrawLineInternal(p5, p6);
            DrawLineInternal(p6, p7);
            DrawLineInternal(p7, p4);
            
            DrawLineInternal(p0, p4);
            DrawLineInternal(p1, p5);
            DrawLineInternal(p2, p6);
            DrawLineInternal(p3, p7);
        }

        public void DrawWireSphereInternal(Vector3 p1, float radius)
        {
            DrawWireDiscInternal(p1, Vector3.up, radius);
            DrawWireDiscInternal(p1, Vector3.right, radius);
        }

        private void DrawCapsuleInternal(Vector3 p1, Vector3 p2, float radius)
        {
             // Special case when both points are in the same position
            if (p1 == p2)
            {
                // DrawWireSphere works only in gizmo methods
                DrawWireSphereInternal(p1, radius);
                return;
            }


            Quaternion p1Rotation = Quaternion.LookRotation(p1 - p2);
            Quaternion p2Rotation = Quaternion.LookRotation(p2 - p1);
            // Check if capsule direction is collinear to Vector.up
            float c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
            if (c == 1f || c == -1f)
            {
                // Fix rotation
                p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
            }

            // First side
            DrawWireArcInternal(p1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
            DrawWireArcInternal(p1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
            DrawWireDiscInternal(p1, (p2 - p1).normalized, radius);
            // Second side
            DrawWireArcInternal(p2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
            DrawWireArcInternal(p2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
            DrawWireDiscInternal(p2, (p1 - p2).normalized, radius);
            // Lines
            DrawLineInternal(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
            DrawLineInternal(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
            DrawLineInternal(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
            DrawLineInternal(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
        }

        public void DrawWireCircle(Vector3 from, Vector3 normal, float radius)
        {
            BeforeDraw();
            DrawWireDiscInternal(from, normal, radius);
            AfterDraw();
        }

        private void DrawWireDiscInternal(Vector3 center, Vector3 normal, float radius)
        {
            var from = Vector3.Cross(normal, Vector3.up);
            if (from.sqrMagnitude < 1.0 / 1000.0)
                from = Vector3.Cross(normal, Vector3.right);
            DrawWireArcInternal(center, normal, from, 360f, radius);
        }

        private void DrawWireArcInternal(Vector3 center, Vector3 normal, Vector3 from, float arc, float radius)
        {
            var segments = 32;

            var segmentAngle = arc / segments;
            
            for (int i = 0; i < segments; i++)
            {
                var current = i + 0;
                var next = i + 1;

                var radVector = from.normalized * radius;
                var p0 = center + Quaternion.AngleAxis(segmentAngle * current, normal) * radVector;
                var p1 = center + Quaternion.AngleAxis(segmentAngle * next, normal) * radVector;
                
                DrawLineInternal(p0, p1);
            }
        }
    }
}