using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    [RequireComponent(typeof(BezierSpline))]
    public class BezierMesh : MonoBehaviour
    {
        BezierSpline spline;

        public int resolution = 10;
        public float radius = 0.5f;
        public int sides = 6;




        void Start()
        {
            spline = GetComponent<BezierSpline>();
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = CreateMesh();
        }



        Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uv = new List<Vector2>();

            DrawPolygonFace(spline.GetPoint(0f), spline.GetDirection(0f), ref vertices, ref triangles);
            DrawPolygonFace(spline.GetPoint(1f), spline.GetDirection(1f), ref vertices, ref triangles, true);





            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();


            return mesh;

        }

        void DrawPolygonFace(Vector3 center, Vector3 normal, ref List<Vector3> vertices, ref List<int> triangles, bool flip = false)
        {
            if (flip) normal = -normal;
            Vector3 offset = new Vector3(radius, 0f, 0f);

            float deltaAngle = 360f / sides;

            vertices.Add(center);

            int centerIndex = vertices.Count - 1;

            for (int i = 0; i <= sides + 1; i++)
            {

                float angle = deltaAngle * i;
                Quaternion rotation = Quaternion.AngleAxis(angle, normal);

                Vector3 vertexPos = center + rotation * offset;


                vertices.Add(vertexPos);
                if (i > 0)
                {
                    triangles.AddRange(new int[] { centerIndex, centerIndex + i, centerIndex + i - 1 });
                }
            }

        }




    }
}
