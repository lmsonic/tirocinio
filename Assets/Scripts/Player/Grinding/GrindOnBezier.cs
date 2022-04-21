using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Tirocinio
{
    public class GrindOnBezier : MonoBehaviour
    {
        List<BezierSpline> splines;

        BezierSpline closestSpline;

        public float grindDistanceThreshold = 0.5f;
        PlayerMovement player;
        void Start()
        {
            player = GetComponent<PlayerMovement>();
            splines = new List<BezierSpline>(FindObjectsOfType<BezierSpline>());
            closestSpline = FindClosestSpline();
        }

        BezierSpline FindClosestSpline()
        {
            return splines.Where(s => s != null)
                .OrderBy(s => (s.GetClosestPoint(transform.position).Item1 - transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void FixedUpdate()
        {
            closestSpline = FindClosestSpline();
            if (closestSpline == null) return;

            (Vector3 closestPoint, float t) = closestSpline.GetClosestPoint(transform.position);

            if ((closestPoint - transform.position).magnitude < grindDistanceThreshold)
            {
                transform.position = closestPoint;
                player.Velocity = closestSpline.GetVelocity(t);
            }
        }








    }
}
