using System.Collections;
using System.Collections.Generic;
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

        public float forceMultiplier = 10f;

        PlayerInput playerInput;

        ObjectPool<PoolObject> seedBombPool;

        public float shootTime = 0.5f;
        float timer = 0f;

        LineRenderer lineRenderer;
        [Range(0f, 100f)]
        public float lineLength = 10f;


        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            lineRenderer = GetComponent<LineRenderer>();
            seedBombPool = new ObjectPool<PoolObject>(seedBombPrefab, 10);

        }





        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (playerInput.IsShooting && timer > shootTime)
            {
                Shoot();
                timer = 0f;
            }
            Vector3 force = (transform.forward + transform.up).normalized * forceMultiplier;
            SimulatePath(transform.position, force, 0f);
        }



        void Shoot()
        {

            GameObject go = seedBombPool.PullGameObject(shootingOrigin.position);
            Rigidbody rb = go.GetComponent<Rigidbody>();

            Vector3 force = (transform.forward + transform.up).normalized * forceMultiplier;
            rb.AddForce(force, ForceMode.VelocityChange);

        }

        private Vector3[] segments;
        private int maxSegmentCount = 40;
        private int numSegments = 0;
        private int maxIterations = 100;
        private float segmentStepModulo = 5f;





        private void SimulatePath(Vector3 startPos, Vector3 forceDirection, float drag)
        {
            float timestep = Time.fixedDeltaTime;

            float currentLineLength = 0f;

            float stepDrag = 1 - drag * timestep;
            Vector3 velocity = forceDirection * timestep;
            Vector3 gravity = Physics.gravity * timestep * timestep;
            Vector3 position = startPos;

            if (segments == null || segments.Length != maxSegmentCount)
            {
                segments = new Vector3[maxSegmentCount];
            }

            segments[0] = position;
            numSegments = 1;

            for (int i = 0; i < maxIterations && numSegments < maxSegmentCount && currentLineLength < lineLength; i++)
            {
                velocity += gravity;
                velocity *= stepDrag;

                position += velocity;

                currentLineLength += velocity.magnitude;

                if (i % segmentStepModulo == 0)
                {
                    segments[numSegments] = position;
                    numSegments++;
                }
            }

            Draw();
        }

        private void Draw()
        {

            lineRenderer.transform.position = segments[0];


            lineRenderer.positionCount = numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                lineRenderer.SetPosition(i, segments[i]);
            }
        }
    }
}
