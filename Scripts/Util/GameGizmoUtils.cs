using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class GameGizmoUtils
    {
        public static GameObject DrawArrow(Vector3 from, Vector3 to)
        {
            return DrawArrow(from, to, from.Distance(to)/4, Color.blue, Color.red);
        }
        
        public static GameObject DrawArrow(Vector3 from, Vector3 to, float arrowHeadLength, Color bottom, Color top, float killDelay = 0f)
        {
            var go = new GameObject("Arrow");
            go.AddComponent<MeshFilter>().mesh = BuildArrowMesh(to.Distance(from), arrowHeadLength, bottom, top);
            go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Vertex Color"));
            go.transform.position = from;
            go.transform.LookAt(to);
            go.transform.rotation *= Quaternion.Euler(90, 0, 0);

            if (killDelay > 0)
            {
                Object.Destroy(go, killDelay);
            }
            return go;
        }

        private static Mesh BuildArrowMesh(float distance, float arrowHeadLength, Color bottom, Color top)
        {
            var arrowHeadRadius = arrowHeadLength / 2.5f;
            var arrowLength = distance - arrowHeadLength;
            var segments = 32;
            var lineRadius = arrowHeadRadius / 4;

            var points = new List<Vector3>();
            var segment = Mathf.PI * 2 / segments;
            for (int i = 0; i < segments; i++)
            {
                var x = Mathf.Cos(segment * i) * arrowHeadRadius;
                var y = Mathf.Sin(segment * i) * arrowHeadRadius;
                points.Add(new Vector3(x, arrowLength, y));
            }

            var meshBuilder = new MeshBuilder();

            // arrow head
            meshBuilder.AddCircle(points.ToArray(), new Vector3(0, arrowLength + arrowHeadLength, 0));
            meshBuilder.AddCircle(points.ToArray(), new Vector3(0, arrowLength, 0), true);

            var points2 = new List<Vector3>();
            for (int i = 0; i < segments; i++)
            {
                var x = Mathf.Cos(segment * i) * lineRadius;
                var y = Mathf.Sin(segment * i) * lineRadius;
                points2.Add(new Vector3(x, 0, y));
            }

            var pointsBottom = points2.Select(p => new Vector3(p.x, arrowLength, p.z)).ToArray();
            meshBuilder.AddStrip(points2.ToArray(), pointsBottom);
            meshBuilder.AddCircle(points2.ToArray(), new Vector3(0, 0, 0), true);
            var buildArrow = meshBuilder.GetMesh();
            buildArrow.colors = buildArrow.vertices.Select(v => Color.Lerp(bottom, top, v.y / distance)).ToArray();
            return buildArrow;
        }
    }
}