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
                if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, mask, QueryTriggerInteraction.Ignore))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }

            public bool Cast(LayerMask mask, out RaycastHit hit)
            {
                if (Physics.Raycast(origin, direction, out hit, distance, mask, QueryTriggerInteraction.Ignore))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }

            public bool SphereCast(float radius, LayerMask mask)
            {
                if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, mask, QueryTriggerInteraction.Ignore))
                {
                    distance = hit.distance;
                    return true;
                }
                return false;
            }

            public bool SphereCast(float radius, LayerMask mask, out RaycastHit hit)
            {
                if (Physics.SphereCast(origin, radius, direction, out hit, distance - radius, mask, QueryTriggerInteraction.Ignore))
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

        [Range(0, 180f)]
        public float wallAngle = 80f;
        [Range(0, 180f)]
        public float ceilingAngle = 120f;


        [Header("Collider Options")]
        [SerializeField]
        public float colliderHeight = 2f;
        public float colliderThickness = 1f;
        public Vector3 colliderOffset = new Vector3(0f, 0.5f, 0f);
        [Header("Sensor Options")]
        public SensorType sensorType = SensorType.Raycast;
        public LayerMask collisionLayers;
        [Range(0.5f, 1f)]
        public float sensorRange = 0.55f;
        public float sphereCastRadius = 0.5f;
        public bool isInDebugMode = false;
        [Header("Sensor Array Options")]
        [Range(1, 5)]
        public int sensorArrayRows;
        [Range(1, 15)]
        public int sensorArrayRayCount;
        public bool sensorArrayRowsAreOffset;

        Vector3 groundPoint;
        Vector3 groundNormal;

        public Vector3 GetGroundPoint() => groundPoint;
        public float GetGroundAngle() => Vector3.Angle(Vector3.up, groundNormal);
        public Vector3 GetGroundNormal() => groundNormal;


        private CapsuleCollider _collider;
        private Rigidbody _rigidbody;

        Raycast currentRaycast;
        List<Raycast> raycastArray = new List<Raycast>();

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
        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }







        bool _isGrounded = false;


        public bool IsGrounded() => _isGrounded;



        public void CheckForGround()
        {
            _isGrounded = false;


            Vector3 origin = transform.position + transform.rotation * colliderOffset;
            RaycastHit hit;
            switch (sensorType)
            {
                case SensorType.Raycast:
                    currentRaycast = new Raycast(origin, -transform.up, colliderHeight * sensorRange);

                    _isGrounded = currentRaycast.Cast(collisionLayers, out hit);
                    if (_isGrounded)
                    {
                        groundPoint = hit.point;
                        groundNormal = hit.normal;
                    }

                    break;
                case SensorType.Spherecast:
                    currentRaycast = new Raycast(origin, -transform.up, colliderHeight * sensorRange);

                    _isGrounded = currentRaycast.SphereCast(sphereCastRadius, collisionLayers, out hit);
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



            Raycast ray = new Raycast(center, -transform.up, colliderHeight * sensorRange);
            //center ray
            if (ray.Cast(collisionLayers, out RaycastHit hit))
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
                    Vector3 worldPos = center + transform.rotation * circlePos * distance;

                    ray = new Raycast(worldPos, -transform.up, colliderHeight * sensorRange);

                    if (ray.Cast(collisionLayers, out RaycastHit hit2))
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


        Vector3 _velocity = Vector3.zero;

        public void SetVelocity(Vector3 velocity)
        {
            _velocity = velocity;
        }

        public Vector3 GetVelocity()
        {
            return _velocity;
        }
        Vector3 targetPosition = Vector3.zero;


        private void FixedUpdate()
        {
            targetPosition = _rigidbody.position;

            MoveAndSlide(_velocity);

            _rigidbody.MovePosition(targetPosition);
        }

        void Depenetrate()
        {
            Collider[] neighbours = new Collider[5];
            float max = Mathf.Max(colliderHeight, colliderThickness);
            int count = Physics.OverlapSphereNonAlloc(transform.position, max, neighbours, collisionLayers, QueryTriggerInteraction.Ignore);


            for (int i = 0; i < count; ++i)
            {
                var collider = neighbours[i];

                if (collider == _collider)
                    continue; // skip ourself

                Vector3 otherPosition = collider.gameObject.transform.position;
                Quaternion otherRotation = collider.gameObject.transform.rotation;

                Vector3 direction;
                float distance;

                bool overlapped = Physics.ComputePenetration(
                    _collider, transform.position, transform.rotation,
                    collider, otherPosition, otherRotation,
                    out direction, out distance
                );

                if (overlapped)
                    targetPosition += direction * distance * 1.5f;


            }
        }




        List<Vector3> slidePositions = new List<Vector3>();


        CollisionInfo MoveAndCollide(Vector3 movement)
        {
            RaycastHit hitInfo;

            float safeDistance;



            Vector3 direction = movement.normalized;
            float distance = movement.magnitude;
            if (_rigidbody.SweepTest(direction, out hitInfo, distance, QueryTriggerInteraction.Ignore)
                && collisionLayers.Contains(hitInfo.transform.gameObject.layer))
            {


                Vector3 closestPoint = _collider.ClosestPoint(hitInfo.point);
                safeDistance = (closestPoint - hitInfo.point).magnitude - 0.08f;
                targetPosition += direction * safeDistance;

                direction = Vector3.ProjectOnPlane(direction, hitInfo.normal);
                distance -= safeDistance;

                slidePositions.Add(targetPosition);


                return new CollisionInfo(hitInfo.point, hitInfo.normal, direction * distance);
            }
            else
            {
                targetPosition += movement;
                return null;
            }

        }







        void MoveAndSlide(Vector3 velocity, int maxSlides = 4, float slopeLimit = 45f)
        {
            slidePositions = new List<Vector3>();

            Vector3 movement = velocity * Time.fixedDeltaTime;



            for (int i = 0; i < maxSlides; i++)
            {

                CollisionInfo info = MoveAndCollide(movement);
                if (info == null) break;

                float angle = Vector3.Angle(Vector3.up, info.normal);
                if (angle < wallAngle)
                {//ground
                    movement = info.remainingVelocity;
                }
                else if (angle > wallAngle && angle < ceilingAngle)
                {//wall
                    Vector3 perpendicularNormal = Vector3.ProjectOnPlane(info.normal, Vector3.up);
                    movement = Vector3.ProjectOnPlane(movement, perpendicularNormal);
                    velocity = Vector3.ProjectOnPlane(velocity, perpendicularNormal);

                }
                else
                {//ceiling
                    Vector3 perpendicularNormal = Vector3.ProjectOnPlane(info.normal, Vector3.up);
                    movement = Vector3.ProjectOnPlane(movement, perpendicularNormal);
                    movement.y = 0f;

                    velocity = Vector3.ProjectOnPlane(velocity, perpendicularNormal);
                    velocity.y = 0f;
                }



                Depenetrate();


            }

            Depenetrate();


            _velocity = velocity;


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
                            DrawCross(groundPoint, 0.2f);
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
                            Gizmos.color = Color.green;
                            DrawCross(groundPoint, 0.2f);

                            Gizmos.color = Color.blue;
                            Gizmos.DrawRay(groundPoint, groundNormal);
                        }
                        break;
                }

                Gizmos.color = Color.blue;

                Gizmos.DrawRay(origin + transform.rotation * colliderOffset, _rigidbody.velocity);

                for (int i = 0; i < slidePositions.Count; i++)
                {
                    Vector3 offsetPosition = slidePositions[i] + transform.rotation * colliderOffset;
                    DrawCapsule(offsetPosition, colliderHeight, colliderThickness);
                    if (i + 1 < slidePositions.Count)
                        Gizmos.DrawLine(offsetPosition, slidePositions[i + 1] + colliderOffset);
                }



            }
        }

        void DrawCapsule(Vector3 center, float height, float radius)
        {

            float sphereOffset = height * 0.5f - radius;

            Vector3 top = center + transform.up * (sphereOffset);
            Vector3 bottom = center - transform.up * (sphereOffset);


            Gizmos.DrawWireSphere(top, radius);
            Gizmos.DrawWireSphere(bottom, radius);

            Gizmos.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius);
            Gizmos.DrawLine(top + Vector3.back * radius, bottom + Vector3.back * radius);
            Gizmos.DrawLine(top + Vector3.right * radius, bottom + Vector3.right * radius);
            Gizmos.DrawLine(top + Vector3.left * radius, bottom + Vector3.left * radius);


        }

        void DrawCross(Vector3 point, float size)
        {

            Vector3 top = new Vector3(point.x, point.y + size, point.z);
            Vector3 bottom = new Vector3(point.x, point.y - size, point.z);
            Gizmos.DrawLine(top, bottom);

            Vector3 forward = new Vector3(point.x, point.y, point.z + size);
            Vector3 backward = new Vector3(point.x, point.y, point.z - size);
            Gizmos.DrawLine(forward, backward);

            Vector3 right = new Vector3(point.x + size, point.y, point.z);
            Vector3 left = new Vector3(point.x - size, point.y, point.z);
            Gizmos.DrawLine(right, left);


        }
    }




}

