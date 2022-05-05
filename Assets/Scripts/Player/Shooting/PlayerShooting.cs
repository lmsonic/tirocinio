
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(LineRenderer))]
    public class PlayerShooting : MonoBehaviour
    {
        public GameObject seedBombPrefab;


        public Transform shootingOrigin;

        public float angleTrajectory = 45f;

        public float forceTrajectory = 10f;

        public PlayerMovement player;

        PlayerInput playerInput;

        ObjectPool<PoolObject> seedBombPool;

        public float shootTime = 0.5f;
        float timer = 0f;

        LineRenderer lineRenderer;
        [Header("Line Trajectory")]
        [Range(0f, 100f)]
        public float lineLength = 10f;

        [Range(1, 50)]
        public int lineResolution = 10;
        public LayerMask plantableMask;
        public LayerMask hitMask;
        public Color plantableColor = Color.green;
        public Color dryColor = Color.gray;

        Transform cameraTransform;


        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            cameraTransform = Camera.main.transform;
            lineRenderer = GetComponent<LineRenderer>();
            seedBombPool = new ObjectPool<PoolObject>(seedBombPrefab, 100);

        }

        Vector3 AngledInitialVelocity(float angle)
        {
            angle -= cameraTransform.eulerAngles.x;
            angle *= Mathf.Deg2Rad;

            return (transform.forward * Mathf.Cos(angle) + transform.up * Mathf.Sin(angle)).normalized;
        }


        Vector3 CalculateForce() => AngledInitialVelocity(angleTrajectory) * forceTrajectory;






        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (playerInput.IsShooting && timer > shootTime)
            {
                Shoot();
                timer = 0f;
            }
            Vector3 force = CalculateForce();
            SimulatePath(transform.position, force, 0f);
        }



        void Shoot()
        {

            GameObject go = seedBombPool.PullGameObject(shootingOrigin.position);
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            Vector3 force = CalculateForce();
            rb.AddForce(force, ForceMode.VelocityChange);

        }

        private Vector3[] segments;
        private int numSegments = 0;
        private int maxIterations = 200;


        private void SimulatePath(Vector3 startPos, Vector3 forceDirection, float drag)
        {
            float timestep = Time.fixedDeltaTime;

            float segmentLength = lineLength / (float)lineResolution;

            float currentLineLength = 0f;
            float currentSegmentLength = 0f;

            bool isPlantable = false;


            float stepDrag = 1 - drag * timestep;
            Vector3 velocity = forceDirection * timestep;
            Vector3 gravity = Physics.gravity * timestep * timestep;
            Vector3 position = startPos;

            if (segments == null || segments.Length != lineResolution)
            {
                segments = new Vector3[lineResolution + 1];
            }

            segments[0] = position;
            numSegments = 1;
            RaycastHit hit;

            for (int i = 0; i < maxIterations && numSegments < lineResolution && currentLineLength < lineLength; i++)
            {
                velocity += gravity;
                velocity *= stepDrag;


                position += velocity;

                currentLineLength += velocity.magnitude;
                currentSegmentLength += velocity.magnitude;


                if (currentSegmentLength >= segmentLength)
                {
                    segments[numSegments] = position;
                    numSegments++;
                    currentSegmentLength = 0f;
                }

                if (Physics.Raycast(position, velocity, out hit, velocity.magnitude, hitMask))
                {
                    isPlantable = plantableMask.Contains(hit.collider.gameObject.layer);
                    segments[numSegments] = hit.point;
                    numSegments++;
                    break;
                }
            }

            Draw(isPlantable);
        }

        private void Draw(bool isPlantable)
        {
            Color lineColor = isPlantable ? plantableColor : dryColor;

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.transform.position = segments[0];


            lineRenderer.positionCount = numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                lineRenderer.SetPosition(i, segments[i]);
            }
        }
    }
}
