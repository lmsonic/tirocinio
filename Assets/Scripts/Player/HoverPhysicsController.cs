using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    public class HoverPhysicsController : MonoBehaviour
    {
        float gravity = -Physics.gravity.y;
        [Header("Hover Variables")]
        public Transform[] anchors;
        float[] lastHitDistances;



        public float sensorRange = 3f;
        public float neutralLength = 3f;
        public float strenght = 3f;
        public float dampening = 3f;
        [Header("Movement Variables")]
        public float acceleration = 5f;
        [Header("Rotation Variables")]
        public float rotationSpeed = 5f;


        Rigidbody rb;

        PlayerInput playerInput;

        private void OnEnable()
        {
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }

        private void Awake()
        {
            playerInput = new PlayerInput();

            playerInput.Player.Accelerate.started += ctx => accelerationInput = ctx.ReadValue<float>();
            playerInput.Player.Accelerate.performed += ctx => accelerationInput = ctx.ReadValue<float>();
            playerInput.Player.Accelerate.canceled += ctx => accelerationInput = ctx.ReadValue<float>();

            playerInput.Player.Move.started += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerInput.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerInput.Player.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            lastHitDistances = new float[anchors.Length];


        }

        float accelerationInput;

        Vector2 movementInput;



        private void FixedUpdate()
        {

            for (int i = 0; i < anchors.Length; i++)
            {
                ApplyHoverForce(i);
            }


            rb.AddRelativeForce(accelerationInput * acceleration * Vector3.forward,ForceMode.Acceleration);

            float cameraAngle = Camera.main.transform.eulerAngles.y;

            rb.AddRelativeTorque(0f,movementInput.x * rotationSpeed,0f,ForceMode.VelocityChange);
            

        }

        void ApplyHoverForce(int i)
        {
            Transform t = anchors[i];
            if (Physics.Raycast(t.position, t.TransformDirection(-Vector3.up), out RaycastHit hit, sensorRange))
            {
                float forceAmount = HooksLawDampen(hit.distance, i);
                rb.AddForceAtPosition(t.up * forceAmount, t.position);

            }
            else
            {
                lastHitDistances[i] = neutralLength * 1.1f;

            }

        }




        float HooksLawDampen(float hitDistance, int i)
        {
            float forceAmount = strenght * (neutralLength - hitDistance) + dampening * (lastHitDistances[i] - hitDistance);
            forceAmount = Mathf.Max(0f, forceAmount);
            lastHitDistances[i] = hitDistance;

            return forceAmount;
        }




    }
}
