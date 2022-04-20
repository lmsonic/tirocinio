using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class ClosestPointTest : MonoBehaviour
    {
        public BezierSpline spline;

        // Update is called once per frame
        private void OnDrawGizmos()
        {
            if (!spline) return;
            (float a, float b) = spline.FindClosestInterval(transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spline.GetPoint(a), 0.1f);
            Gizmos.DrawSphere(spline.GetPoint(b), 0.1f);

            Gizmos.color = Color.magenta;
            Vector3 closestPoint = spline.GetClosestPoint(transform.position).Item1;
            Gizmos.DrawLine(transform.position, closestPoint);
            Gizmos.DrawSphere(closestPoint, 0.3f);



        }
    }
}
