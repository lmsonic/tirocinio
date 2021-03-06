using UnityEngine;
using UnityEngine.Events;
using System;

namespace Tirocinio
{
    public class BezierSpline : MonoBehaviour
    {
        [SerializeField]
        private Vector3[] points;

        [SerializeField]
        private BezierControlPointMode[] modes;

        [SerializeField]
        private bool loop;

        public delegate void Refresh();
        public UnityAction RefreshBezier;


        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
                if (value == true)
                {
                    points[points.Length - 1] = points[0];
                    SetControlPoint(0, points[0]);
                    RefreshBezier.Invoke();
                }
            }
        }

        public int CurveCount
        {
            get
            {
                return (points.Length - 1) / 3;
            }
        }

        public int ControlPointCount
        {
            get
            {
                return points.Length;
            }
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (loop)
                {
                    if (index == 0)
                    {
                        points[points.Length - 1] = point;
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                    }
                    else if (index == points.Length - 1)
                    {
                        points[0] = point;
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                    }
                    else
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                        points[index - 1] += delta;
                    if (index + 1 < points.Length)
                        points[index + 1] += delta;
                }

            }
            points[index] = point;
            EnforceMode(index);
            RefreshBezier.Invoke();
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            modes[modeIndex] = mode;
            if (loop)
            {
                if (modeIndex == 0)
                {
                    modes[modes.Length - 1] = mode;
                }
                if (modeIndex == modes.Length - 1)
                {
                    modes[0] = mode;
                }
            }
            EnforceMode(index);
            RefreshBezier.Invoke();
        }

        void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
                return;

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                    fixedIndex = points.Length - 2;

                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Length)
                    enforcedIndex = 1;

            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Length)
                    fixedIndex = 1;

                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                    enforcedIndex = points.Length - 2;

            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
            }
            points[enforcedIndex] = middle + enforcedTangent;
        }

        public void Reset()
        {
            points = new Vector3[] {
                new Vector3(1f,0f,0f),
                new Vector3(2f,0f,0f),
                new Vector3(3f,0f,0f),
                new Vector3(4f,0f,0f),
            };
            modes = new BezierControlPointMode[]{
                BezierControlPointMode.Free,
                BezierControlPointMode.Free,
            };
            RefreshBezier.Invoke();
        }

        public void AddCurve()
        {
            Vector3 point = points[points.Length - 1];
            Array.Resize(ref points, points.Length + 3);
            point.x += 1f;
            points[points.Length - 3] = point;
            point.x += 1f;
            points[points.Length - 2] = point;
            point.x += 1f;
            points[points.Length - 1] = point;

            Array.Resize(ref modes, modes.Length + 1);
            modes[modes.Length - 1] = modes[modes.Length - 2];
            EnforceMode(points.Length - 4);

            if (loop)
            {
                points[points.Length - 1] = points[0];
                modes[modes.Length - 1] = modes[0];
                EnforceMode(0);

            }
            RefreshBezier.Invoke();
        }


        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }





        public (Vector3, float) GetClosestPoint(Vector3 point)
        {


            (float a, float b) = FindClosestInterval(point);

            float t = FindClosestPointBisection(a, b, point);

            return (GetPoint(t), t);
        }

        const float maxIterations = 20;
        const float precision = 1e-3f;


        float FindClosestPointBisection(float a, float b, Vector3 point)
        {
            float c = (a + b) / 2;
            int k = 0;
            while (b - a > precision && k < maxIterations)
            {
                c = (a + b) / 2;

                float distA = DistanceFromPointOnBezier(point, a);
                float distB = DistanceFromPointOnBezier(point, b);
                float distC = DistanceFromPointOnBezier(point, c);

                if (distC < distA)
                    a = c;
                else if (distC < distB)
                    b = c;

                k++;
            }

            return c;
        }



        const int samples = 20;

        public (float a, float b) FindClosestInterval(Vector3 point)
        {
            float stepSize = 1f / samples;

            float closestPoint = 0f;
            float minDistance = Mathf.Infinity;
            for (int i = 0; i <= samples; i++)
            {
                float distance = DistanceFromPointOnBezier(point, stepSize * i);
                if (distance < minDistance)
                {
                    closestPoint = stepSize * i;
                    minDistance = distance;
                }
            }

            float previousDistance = DistanceFromPointOnBezier(point, closestPoint - stepSize);
            float nextDistance = DistanceFromPointOnBezier(point, closestPoint + stepSize);

            float otherPoint = closestPoint + stepSize;
            if (previousDistance < nextDistance)
            {
                float tmp = closestPoint;
                closestPoint = closestPoint - stepSize;
                otherPoint = tmp;
            }



            return (closestPoint, otherPoint);

        }

        float DistanceFromPointOnBezier(Vector3 point, float t)
        {
            return (point - GetPoint(t)).magnitude;
        }







    }
}