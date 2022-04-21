using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Tirocinio
{
    [RequireComponent(typeof(BezierSpline))]
    public class GrindingBezier : MonoBehaviour
    {
        BezierSpline spline;

        public float grindDistance = 0.5f;


        PlayerMovement player;
        void Start()
        {
            spline = GetComponent<BezierSpline>();
            player = FindObjectOfType<PlayerMovement>();

        }

        private void FixedUpdate()
        {
            if (player)
            {
                (Vector3 closestPoint, float t) = spline.GetClosestPoint(player.transform.position);

                if ((closestPoint - player.transform.position).magnitude < grindDistance)
                {
                    player.SetGrindingState(spline);
                }
            }


        }








    }
}
