using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Mover : MonoBehaviour
    {
        public struct Raycast
        {
            public Raycast(Vector3 origin, Vector3 direction, float distance)
            {
                this.origin = origin;
                this.direction = direction;
                this.distance = distance;
            }
            public Vector3 origin;
            public Vector3 direction;
            public float distance;

            public bool Cast(LayerMask mask)
            {
                if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~mask))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }

            public bool Cast(LayerMask mask, out RaycastHit hit)
            {
                if (Physics.Raycast(origin, direction, out hit, distance, ~mask))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }

            public bool SphereCast(float radius, LayerMask mask)
            {
                if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, ~mask))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }

            public bool SphereCast(float radius, LayerMask mask, out RaycastHit hit)
            {
                if (Physics.SphereCast(origin, radius, direction, out hit, distance, ~mask))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }


        }

        public class CollisionInfo
        {
            public Vector3 position, normal, remainingVelocity;
            public CollisionInfo(Vector3 position, Vector3 normal, Vector3 remainingVelocity)
            {
                this.position = position;
                this.normal = normal;
                this.remainingVelocity = remainingVelocity;
            }

            public float GetAngle(Vector3 upDirection)
            {
                return Vector3.Angle(upDirection, normal);
            }

            public float GetAngle()
            {
                return GetAngle(Vector3.up);
            }


        }

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

        public Mesh capsuleMesh;

        Vector3 groundPoint;
        Vector3 groundNormal;

        public Vector3 GetGroundPoint() => groundPoint;
        public Vector3 GetGroundNormal() => groundNormal;


        private CapsuleCollider _collider;
        private Rigidbody _rigidbody;

        Raycast currentRaycast;
        List<Raycast> raycastArray = new List<Raycast>();

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

        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        float sensorRange = 0.6f;

        const float sphereCastRadius = 0.4f;
        bool _isGrounded = false;


        public bool IsGrounded() => _isGrounded;

        public void CheckForGround()
        {

            Vector3 origin = transform.position + colliderOffset;
            LayerMask mask = gameObject.layer;
            RaycastHit hit;
            switch (sensorType)
            {
                case SensorType.Raycast:
                    currentRaycast = new Raycast(origin, -transform.up, colliderHeight * sensorRange);

                    _isGrounded = currentRaycast.Cast(mask, out hit);
                    if (_isGrounded)
                    {
                        groundPoint = hit.point;
                        groundNormal = hit.normal;
                    }

                    break;
                case SensorType.Spherecast:
                    currentRaycast = new Raycast(origin, -transform.up, colliderHeight * sensorRange);

                    _isGrounded = currentRaycast.SphereCast(sphereCastRadius, mask, out hit);
                    if (_isGrounded)
                    {
                        groundPoint = hit.point;
                        groundNormal = hit.normal;
                    }

                    break;
                case SensorType.RaycastArray:
                    _isGrounded = RaycastArray(origin, colliderThickness, ref groundPoint, ref groundNormal);

                    break;
            }


        }
        bool RaycastArray(Vector3 center, float radius, ref Vector3 position, ref Vector3 normal)
        {
            bool grounded = false;
            List<Vector3> points = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            raycastArray = new List<Raycast>();

            LayerMask mask = gameObject.layer;


            Raycast ray = new Raycast(center, -transform.up, colliderHeight * sensorRange);
            //center ray
            if (ray.Cast(mask, out RaycastHit hit))
            {
                points.Add(hit.point);
                normals.Add(hit.normal);
                grounded = true;
            }
            raycastArray.Add(ray);


            for (int i = 1; i <= sensorArrayRows; i++)
            {
                float distance = ((float)i / sensorArrayRows) * radius;

                for (int j = 0; j < sensorArrayRayCount; j++)
                {
                    float deltaAngle = 360f / sensorArrayRayCount;
                    float angle = j * deltaAngle;

                    if (sensorArrayRowsAreOffset)
                    {
                        bool isRowOdd = i % 2 == 1;
                        if (!isRowOdd)
                        {
                            angle += deltaAngle * 0.5f;
                        }
                    }

                    angle *= Mathf.Deg2Rad;

                    Vector3 circlePos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                    Vector3 worldPos = center + circlePos * distance;

                    ray = new Raycast(worldPos, -transform.up, colliderHeight * sensorRange);

                    if (ray.Cast(mask, out RaycastHit hit2))
                    {
                        points.Add(hit2.point);
                        normals.Add(hit2.normal);
                        grounded = true;
                    }
                    raycastArray.Add(ray);


                }
            }
            if (grounded)
            {
                position = Average(points);
                normal = Average(normals).normalized;
            }

            return grounded;

        }

        Vector3 Average(List<Vector3> vectors)
        {
            Vector3 average = Vector3.zero;

            foreach (Vector3 v in vectors)
            {
                average += v;
            }

            average /= vectors.Count;

            return average;
        }

        public void SetExtendSensorRange(bool value)
        {
            sensorRange = value ? 0.7f : 0.6f;
        }

        Vector3 _velocity;

        public void SetVelocity(Vector3 velocity)
        {
            _velocity = velocity;
        }

        public Vector3 GetVelocity()
        {
            return _velocity;
        }

        Vector3 targetPosition;


        private void FixedUpdate()
        {
            Vector3 lateralVelocity = _velocity;
            lateralVelocity.y = 0f;
            Vector3 verticalVelocity = _velocity;
            verticalVelocity.x = 0f; verticalVelocity.z = 0f;

            targetPosition = transform.position;

            MoveAndSlide(lateralVelocity * Time.fixedDeltaTime);

            MoveAndSlide(verticalVelocity * Time.fixedDeltaTime);

            _rigidbody.MovePosition(targetPosition);
        }

        void MoveAndSlide(Vector3 linearVelocity, int maxSlides = 4)
        {
            MoveAndSlide(linearVelocity, transform.up, maxSlides);
        }

        CollisionInfo MoveAndCollide(Vector3 linearVelocity)
        {

            float capsuleOffset = colliderHeight * 0.5f - colliderThickness;

            Vector3 origin = targetPosition + colliderOffset;
            Vector3 top = origin + transform.up * colliderHeight;
            Vector3 bottom = origin - transform.up * (colliderHeight - StepHeight);


            LayerMask mask = gameObject.layer;

            RaycastHit hitInfo;

            float safeDistance;

            Vector3 direction = linearVelocity.normalized;
            float distance = linearVelocity.magnitude;

            if (_rigidbody.SweepTest(direction, out hitInfo, distance))
            {
                Vector3 closestPoint = _collider.ClosestPoint(hitInfo.point);
                safeDistance = (closestPoint - hitInfo.point).magnitude - 0.08f;
                targetPosition += direction * safeDistance;

                direction = Vector3.ProjectOnPlane(direction, hitInfo.normal);
                distance -= safeDistance;

                KeepGrounded(linearVelocity.y);

                return new CollisionInfo(hitInfo.point, hitInfo.normal, direction * distance);
            }
            else
            {
                targetPosition += linearVelocity;
                KeepGrounded(linearVelocity.y);
                return null;
            }

            // if (Physics.CapsuleCast(top, bottom, colliderThickness, direction, out hitInfo, distance + colliderThickness, ~mask))
            // {
            //     Vector3 closestPoint = _collider.ClosestPoint(hitInfo.point);
            //     safeDistance = hitInfo.distance - colliderThickness - 0.08f;
            //     targetPosition += direction * safeDistance;

            //     direction = Vector3.ProjectOnPlane(direction, hitInfo.normal);
            //     distance -= safeDistance;

            //     KeepGrounded(linearVelocity.y);

            //     return new CollisionInfo(hitInfo.point, hitInfo.normal, direction * distance);
            // }
            // else
            // {
            //     targetPosition += linearVelocity;

            //     KeepGrounded(linearVelocity.y);


            //     return null;
            // }
        }

        void KeepGrounded(float verticalVelocity)
        {
            if (_isGrounded)
            {
                if (verticalVelocity == 0f)
                    targetPosition.y = groundPoint.y;
                else    
                    targetPosition.y = Mathf.Lerp(targetPosition.y,groundPoint.y,Time.fixedDeltaTime  *10f);
            }
        }



        void MoveAndSlide(Vector3 linearVelocity, Vector3 upDirection, int maxSlides = 4)
        {

            LayerMask mask = gameObject.layer;



            for (int i = 0; i < maxSlides; i++)
            {


                CollisionInfo info = MoveAndCollide(linearVelocity);
                if (info == null) break;

                linearVelocity = info.remainingVelocity;


            }

        }



        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position + colliderOffset;

            if (isInDebugMode && Application.isPlaying)
            {
                switch (sensorType)
                {
                    case SensorType.Raycast:
                        Gizmos.color = Color.red;
                        Gizmos.DrawRay(currentRaycast.origin, currentRaycast.direction * currentRaycast.distance);
                        if (_isGrounded)
                        {
                            Gizmos.color = Color.green;
                            DrawGreenCross(groundPoint, 0.2f);
                            Gizmos.color = Color.blue;
                            Gizmos.DrawRay(groundPoint, groundNormal);
                        }


                        break;
                    case SensorType.Spherecast:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(currentRaycast.origin, sphereCastRadius);
                        Gizmos.DrawRay(currentRaycast.origin, currentRaycast.direction * currentRaycast.distance);

                        if (_isGrounded)
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawRay(groundPoint, groundNormal);

                            Gizmos.color = Color.green;
                        }

                        Gizmos.DrawWireSphere(currentRaycast.origin + currentRaycast.direction * currentRaycast.distance, sphereCastRadius);


                        break;
                    case SensorType.RaycastArray:
                        Gizmos.color = Color.red;
                        foreach (Raycast raycast in raycastArray)
                        {
                            Gizmos.DrawRay(raycast.origin, raycast.direction * raycast.distance);
                        }

                        if (_isGrounded)
                        {
                            DrawGreenCross(groundPoint, 0.2f);

                            Gizmos.color = Color.blue;
                            Gizmos.DrawRay(groundPoint, groundNormal);
                        }
                        break;
                }

                Gizmos.color = Color.blue;

                Gizmos.DrawRay(origin, _rigidbody.velocity);

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
    }




}

