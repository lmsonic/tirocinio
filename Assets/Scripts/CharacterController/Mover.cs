using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Mover : MonoBehaviour
    {
        public enum SensorType { Raycast, Spherecast, RaycastArray }
        [Header("Mover Options")]
        [Range(0, 1)]
        public float stepHeightRatio = 0.25f;
        [Header("Collider Options")]
        [SerializeField]
        public float colliderHeight = 2f;
        public float colliderThickness = 1f;
        public Vector3 colliderOffset = new Vector3(0f, 0.5f, 0f);
        [Header("Sensor Options")]
        public SensorType sensorType = SensorType.Raycast;
        public bool isInDebugMode = false;
        [Header("Sensor Array Options")]
        [Range(1, 5)]
        public int sensorArrayRows;
        [Range(1, 15)]
        public int sensorArrayRayCount;
        public bool sensorArrayRowsAreOffset;

        private CapsuleCollider _collider;
        private Rigidbody _rigidbody;

        float StepHeight
        {
            get => stepHeightRatio * colliderHeight;
        }

        public void SetColliderHeight(float _newColliderHeight)
        {
            colliderHeight = _newColliderHeight;
            _collider.height = _newColliderHeight;
        }
        public void SetColliderThickness(float _newColliderThickness)
        {
            colliderThickness = _newColliderThickness;
            _collider.radius = _newColliderThickness;
        }
        public void SetStepHeightRatio(float _newStepHeightRatio)
        {
            stepHeightRatio = _newStepHeightRatio;
        }

        const float groundDistance = 0.5f;
        const float raySensorDistance = 0.5f;

        const float sphereCastRadius = 0.4f;
        bool _isGrounded = false;

        Vector3? hitPoint = null;

        public void CheckForGround()
        {
            Vector3 bottom = transform.position + colliderOffset;
            bottom.y -= colliderHeight * 0.5f;
            LayerMask mask = gameObject.layer;
            RaycastHit hit;
            switch (sensorType)
            {
                case SensorType.Raycast:
                    if (Physics.Raycast(bottom, -transform.up, out hit, groundDistance, ~mask))
                    {
                        _isGrounded = true;
                        hitPoint = hit.point;
                    }
                    else hitPoint = null;
                    break;
                case SensorType.Spherecast:
                    if (Physics.SphereCast(bottom, sphereCastRadius, -transform.up, out hit, groundDistance, ~mask))
                    {
                        _isGrounded = true;
                        hitPoint = hit.point;
                    }
                    else hitPoint = null;

                    break;
            }

        }

        void DrawGreenCross(Vector3 point, float size)
        {

            Vector3 top = new Vector3(point.x, point.y + size, point.z);
            Vector3 bottom = new Vector3(point.x, point.y - size, point.z);
            Debug.DrawLine(top, bottom, Color.green);

            Vector3 forward = new Vector3(point.x, point.y, point.z + size);
            Vector3 backward = new Vector3(point.x, point.y, point.z - size);
            Debug.DrawLine(forward, backward, Color.green);

            Vector3 right = new Vector3(point.x + size, point.y, point.z);
            Vector3 left = new Vector3(point.x - size, point.y, point.z);
            Debug.DrawLine(right, left, Color.green);


        }

        public bool IsGrounded()
        {
            return false;
        }

        public void SetExtendSensorRange(bool value)
        {

        }

        public void SetVelocity(Vector3 velocity)
        {

        }



        private void OnDrawGizmos()
        {
            Vector3 bottom = transform.position + colliderOffset;
            bottom.y -= colliderHeight * 0.5f;
            if (isInDebugMode)
            {
                switch (sensorType)
                {
                    case SensorType.Raycast:
                        Gizmos.color = Color.red;
                        if (!_isGrounded || hitPoint == null)
                        {
                            Gizmos.DrawRay(bottom,-transform.up * groundDistance);
                        }
                        else
                        {
                            Gizmos.DrawLine(bottom, (Vector3)hitPoint);
                            Gizmos.color = Color.green;
                            DrawGreenCross((Vector3)hitPoint, 0.2f);
                        }
                        break;
                    case SensorType.Spherecast:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(bottom, sphereCastRadius);
                        if (!_isGrounded || hitPoint == null)
                        {
                            Gizmos.DrawRay(bottom, transform.up * groundDistance);
                            Gizmos.DrawWireSphere(bottom - transform.up * groundDistance, sphereCastRadius);
                        }
                        else
                        {
                            Gizmos.DrawLine(bottom, (Vector3)hitPoint);
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireSphere((Vector3)hitPoint, sphereCastRadius);
                        }
                        break;
                }
            }
        }




    }
}
