using UnityEngine;
using System;

namespace Tirocinio
{
    // Finds the slope/grade/incline angle of ground underneath a CharacterController
    public class GroundChecker : MonoBehaviour
    {

        [Header("Results")]
        public float groundSlopeAngle = 0f;            // Angle of the slope in degrees
        public Vector3 groundSlopeNormal = Vector3.zero;  // The calculated slope as a vector

        public bool isGrounded = false;


        [Header("Settings")]
        public bool showDebug = false;                  // Show debug gizmos and lines
        public LayerMask castingMask;                  // Layer mask for casts. You'll want to ignore the player.
        public float sphereCastRadius = 0.25f;
        public float sphereCastDistance = 0.75f;       // How far spherecast moves down from origin point

        public float raycastLength = 0.75f;
        public Transform rayOriginOffset1;
        public Transform rayOriginOffset2;


        public void SetGroundedFalseFor(float seconds)
        {
            isGrounded = false;
            checkGround = false;
            Invoke("ResetCheckGround", seconds);

        }

        void ResetCheckGround()
        {
            checkGround = true;
        }

        bool checkGround = true;



        void FixedUpdate()
        {
            // Check ground, with an origin point defaulting to the bottom middle
            // of the char controller's collider. Plus a little higher 
            if (checkGround)
                isGrounded = CheckGround(transform.position);

        }



        /// <summary>
        /// Checks for ground underneath, to determine some info about it, including the slope angle.
        /// </summary>
        /// <param name="origin">Point to start checking downwards from</param>
        public bool CheckGround(Vector3 origin)
        {
            bool hitGround = false;
            // Out hit point from our cast(s)
            RaycastHit hit;

            // SPHERECAST
            // "Casts a sphere along a ray and returns detailed information on what was hit."
            if (Physics.SphereCast(origin, sphereCastRadius, Vector3.down, out hit, sphereCastDistance, castingMask))
            {
                // Angle of our slope (between these two vectors). 
                // A hit normal is at a 90 degree angle from the surface that is collided with (at the point of collision).
                // e.g. On a flat surface, both vectors are facing straight up, so the angle is 0.
                groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                // Find the vector that represents our slope as well. 
                //  temp: basically, finds vector moving across hit surface 
                //  Now use this vector and the hit normal, to find the other vector moving up and down the hit surface
                groundSlopeNormal = hit.normal;

                hitGround = true;

            }

            // Now that's all fine and dandy, but on edges, corners, etc, we get angle values that we don't want.
            // To correct for this, let's do some raycasts. You could do more raycasts, and check for more
            // edge cases here. There are lots of situations that could pop up, so test and see what gives you trouble.
            RaycastHit slopeHit1;
            RaycastHit slopeHit2;

            // FIRST RAYCAST
            if (Physics.Raycast(rayOriginOffset1.position, Vector3.down, out slopeHit1, raycastLength))
            {
                // Debug line to first hit point
                if (showDebug) { Debug.DrawLine(rayOriginOffset1.position, slopeHit1.point, Color.red); }
                // Get angle of slope on hit normal
                float angleOne = Vector3.Angle(slopeHit1.normal, Vector3.up);

                float distanceToHit1 = (slopeHit1.point - transform.position).magnitude;
                // 2ND RAYCAST
                if (Physics.Raycast(rayOriginOffset2.position, Vector3.down, out slopeHit2, raycastLength))
                {
                    // Debug line to second hit point
                    if (showDebug) { Debug.DrawLine(rayOriginOffset2.position, slopeHit2.point, Color.red); }
                    // Get angle of slope of these two hit points.
                    float angleTwo = Vector3.Angle(slopeHit2.normal, Vector3.up);
                    // 3 collision points: Take the MEDIAN by sorting array and grabbing middle.
                    float[] tempArray = new float[] { groundSlopeAngle, angleOne, angleTwo };
                    Array.Sort(tempArray);
                    groundSlopeAngle = tempArray[1];


                }
                else
                {
                    // 2 collision points (sphere and first raycast): AVERAGE the two
                    float average = (groundSlopeAngle + angleOne) / 2;
                    groundSlopeAngle = average;

                }

                hitGround = true;


            }

            return hitGround;
        }

        void OnDrawGizmosSelected()
        {
            if (showDebug)
            {
                // Visualize SphereCast with two spheres and a line
                Vector3 startPoint = transform.position;
                Vector3 endPoint = transform.position;
                endPoint.y -= sphereCastDistance;

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(startPoint, sphereCastRadius);

                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(endPoint, sphereCastRadius);

                Gizmos.DrawLine(startPoint, endPoint);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, groundSlopeNormal);
            }
        }

    }
}