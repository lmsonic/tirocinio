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

        bool update = false;

        private void OnValidate()
        {
            update = true;
        }

        private void LateUpdate()
        {
            if (update)
            {
                UpdateMesh();
                update = false;
            }

        }




        private void OnEnable()
        {
            spline = GetComponent<BezierSpline>();
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = CreateMesh();
            spline.RefreshBezier += UpdateMesh;
            Undo.undoRedoPerformed += UpdateMesh;
        }



        public void UpdateMesh()
        {

            meshFilter.mesh = CreateMesh();
        }

        private void OnDisable()
        {
            spline.RefreshBezier -= UpdateMesh;
            Undo.undoRedoPerformed -= UpdateMesh;
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
            Vector3 center = spline.GetPoint(t) - transform.position;
            Vector3 normal = spline.GetDirection(t);
            if (flip) normal = -normal;



            float deltaAngle = 360f / sides;

            vertices.Add(center);

            int centerIndex = vertices.Count - 1;

            for (int i = 0; i <= sides + 1; i++)
            {

                float angle = deltaAngle * i;
                Quaternion rotation = Quaternion.AngleAxis(angle, normal);

                Vector3 up = Vector3.ProjectOnPlane(Vector3.up, normal).normalized;

                Vector3 vertexPos = center + (rotation * up) * radius;


                vertices.Add(vertexPos);
                if (i > 0)
                {
                    triangles.AddRange(new int[] { centerIndex, centerIndex + i, centerIndex + i - 1 });
                }
            }

        }




        void GenerateSegment(float startT, float endT, ref List<Vector3> vertices, ref List<int> triangles)
        {

            Vector3 start = spline.GetPoint(startT) - transform.position;
            Vector3 end = spline.GetPoint(endT) - transform.position;

            Vector3 startNormal = spline.GetDirection(startT);
            Vector3 endNormal = spline.GetDirection(endT);




            float deltaAngle = 360f / sides;



            for (int i = 0; i <= sides; i++)
            {


                float angle = deltaAngle * i;
                Quaternion startRotation = Quaternion.AngleAxis(angle, startNormal);
                Quaternion endRotation = Quaternion.AngleAxis(angle, endNormal);

                Vector3 startUp = Vector3.ProjectOnPlane(Vector3.up, startNormal).normalized;
                Vector3 endUp = Vector3.ProjectOnPlane(Vector3.up, endNormal).normalized;



                Vector3 startPos = start + (startRotation * startUp) * radius;
                Vector3 endPos = end + (endRotation * endUp) * radius;

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
