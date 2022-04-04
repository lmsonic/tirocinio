using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tirocinio
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BezierSpline))]
    public class BezierMesh : MonoBehaviour
    {
        BezierSpline spline;

        public int resolution = 10;
        public float radius = 0.5f;
        public int sides = 6;

        MeshFilter meshFilter;





        void Start()
        {
            spline = GetComponent<BezierSpline>();
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = CreateMesh();
            spline.RefreshBezier += UpdateMesh;
        }

        public void UpdateMesh()
        {

            meshFilter.mesh = CreateMesh();
        }



        Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Bezier";
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uv = new List<Vector2>();

            GeneratePolyFace(0f, ref vertices, ref triangles);

            float stepSize = 1f / resolution;
            for (int i = 0; i < resolution; i++)
            {
                Debug.Log(stepSize * i + "," + stepSize * (i + 1));
                GenerateSegment(stepSize * i, stepSize * (i + 1), ref vertices, ref triangles);
            }

            GeneratePolyFace(1f, ref vertices, ref triangles, true);





            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();


            return mesh;

        }

        void GeneratePolyFace(float t, ref List<Vector3> vertices, ref List<int> triangles, bool flip = false)
        {
            Vector3 center = spline.GetPoint(t);
            Vector3 normal = spline.GetDirection(t);
            if (flip) normal = -normal;



            float deltaAngle = 360f / sides;

            vertices.Add(center);

            int centerIndex = vertices.Count - 1;

            for (int i = 0; i <= sides + 1; i++)
            {

                float angle = deltaAngle * i;
                Quaternion rotation = Quaternion.AngleAxis(angle, normal);

                Vector3 vertexPos = center + (rotation * Vector3.up) * radius;


                vertices.Add(vertexPos);
                if (i > 0)
                {
                    triangles.AddRange(new int[] { centerIndex, centerIndex + i, centerIndex + i - 1 });
                }
            }

        }
        struct Quad
        {
            public Vector3 bottomRight, topLeft;

        }



        void GenerateSegment(float startT, float endT, ref List<Vector3> vertices, ref List<int> triangles)
        {

            Vector3 start = spline.GetPoint(startT);
            Vector3 end = spline.GetPoint(endT);

            Vector3 startNormal = spline.GetDirection(startT);
            Vector3 endNormal = spline.GetDirection(endT);




            float deltaAngle = 360f / sides;



            for (int i = 0; i <= sides; i++)
            {


                float angle = deltaAngle * i;
                Quaternion startRotation = Quaternion.AngleAxis(angle, startNormal);
                Quaternion endRotation = Quaternion.AngleAxis(angle, endNormal);


                Vector3 startPos = start + (startRotation * Vector3.up) * radius;
                Vector3 endPos = end + (endRotation * Vector3.up) * radius;

                vertices.Add(startPos);
                vertices.Add(endPos);

                if (i > 0)

                {
                    Vector3 OldStart = vertices[vertices.Count - 4];
                    Vector3 OldEnd = vertices[vertices.Count - 3];




                    int currentIndex = vertices.Count - 1;


                    triangles.AddRange(new int[]{
                        currentIndex - 1, currentIndex , currentIndex - 3,
                    });

                    triangles.AddRange(new int[]{
                        currentIndex - 3, currentIndex , currentIndex - 2,
                    });
                }


            }





        }




    }
}
