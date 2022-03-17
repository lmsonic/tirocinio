using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    public class PlayerStateMachine : MonoBehaviour
    {

        PlayerInput playerInput;
        public Rigidbody Rigidbody { get => body; }

        Rigidbody body;

        public CapsuleCollider groundCapsule;
        public CapsuleCollider forwardCapsule;

        Animator animator;
        Transform cameraTransform;
        public Transform handleTransform;
        public Transform bodyTransform;
        public float maxTurnDegrees = 30f;
        public float rotationSpeed = 30f;
        public float MaxSpeed = 30f;
        public float BackwardsSpeed = 10f;


        public float Gravity { get => gravity; }
        float gravity;

        public float groundDistance = 0.2f;
        public float maxWalkingAngle = 45f;

        public float anglePower = 0.5f;

        Vector2 currentMovementInput;
        [HideInInspector]
        public Vector3 CurrentMovement;
        public float AccelerationInput { get => accelerationInput; }
        float accelerationInput;
        public float BrakeInput { get => brakeInput; }
        float brakeInput;
        public Vector3 Velocity;

        public float AccelerationMultiplier = 2f;
        public float BrakeMultiplier = 3f;
        public float DragMultiplier = 1.5f;


        public bool IsJumpPressed { get => isJumpPressed; }
        bool isJumpPressed = false;
        public bool RequireNewJumpPress { set => requireNewJumpPress = value; get => requireNewJumpPress; }
        bool requireNewJumpPress = false;
        float initialJumpVelocity;

        public float InitialJumpVelocity { get => initialJumpVelocity; }

        public float maxJumpHeight = 1f;
        public float maxJumpTime = 0.5f;
        public float MaxFallSpeed = -30f;
        public float FallMultiplier = 1.5f;

        public PlayerBaseState CurrentState
        {
            set { currentState = value; }
            get { return currentState; }
        }

        PlayerBaseState currentState;

        PlayerStateFactory states;


        private void Awake()
        {
            playerInput = new PlayerInput();
            body = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;



            states = new PlayerStateFactory(this);

            currentState = states.Grounded();
            currentState.EnterState();

            playerInput.Player.Move.started += OnMovementInput;
            playerInput.Player.Move.canceled += OnMovementInput;
            playerInput.Player.Move.performed += OnMovementInput;

            playerInput.Player.Accelerate.started += OnAccelerationInput;
            playerInput.Player.Accelerate.canceled += OnAccelerationInput;
            playerInput.Player.Accelerate.performed += OnAccelerationInput;

            playerInput.Player.Brake.started += OnBrakeInput;
            playerInput.Player.Brake.canceled += OnBrakeInput;
            playerInput.Player.Brake.performed += OnBrakeInput;

            playerInput.Player.Jump.started += OnJump;
            playerInput.Player.Jump.canceled += OnJump;

            SetupJumpVariables();
        }

        void SetupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = 2 * maxJumpHeight / timeToApex;
        }

        void OnJump(InputAction.CallbackContext context)
        {
            isJumpPressed = context.ReadValueAsButton();
            requireNewJumpPress = false;
        }

        void OnAccelerationInput(InputAction.CallbackContext context)
        {
            accelerationInput = context.ReadValue<float>();
        }

        void OnBrakeInput(InputAction.CallbackContext context)
        {
            brakeInput = context.ReadValue<float>();
        }



        void OnMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            CurrentMovement.x = currentMovementInput.x;
            CurrentMovement.z = currentMovementInput.y;
            CurrentMovement.y = 0f;

        }

        void HandleRotation()
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            //rotate handle
            Quaternion targetRotation = Quaternion.Euler(0f, CurrentMovement.x * maxTurnDegrees, 0f);
            handleTransform.localRotation = Quaternion.Slerp(handleTransform.localRotation, targetRotation, Time.deltaTime);
            //turn
            targetRotation = Quaternion.Euler(0f, 0f, -CurrentMovement.x * maxTurnDegrees);
            bodyTransform.localRotation = Quaternion.Slerp(bodyTransform.localRotation, targetRotation, Time.deltaTime);

            //actual rotation
            targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);






        }


        void Update()
        {
            HandleRotation();
            currentState.UpdateStates();
            transform.position = MovePlayer(Velocity * Time.deltaTime);
        }

        

        public void SnapPlayerDown()
        {
            bool closeToGround = CastSelf(
                transform.position,
                transform.rotation,
                Vector3.down,
                0.45f,
                groundCapsule,
                out RaycastHit groundHit);

            // If within the threshold distance of the ground
            if (closeToGround && groundHit.distance > 0)
            {
                // Snap the player down the distance they are from the ground
                transform.position += Vector3.down * (groundHit.distance - 0.001f * 2);
            }
        }

        public Vector3 MovePlayer(Vector3 movement)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            Vector3 remaining = movement;

            int bounces = 0;

            while (bounces < 3 && remaining.magnitude > 0.001f)
            {
                // Do a cast of the collider to see if an object is hit during this
                // movement bounce
                float distance = remaining.magnitude;
                if (!CastSelf(position, rotation, remaining.normalized, distance, forwardCapsule, out RaycastHit hit))
                {
                    // If there is no hit, move to desired position
                    position += remaining;

                    // Exit as we are done bouncing
                    break;
                }

                // If we are overlapping with something, just exit.
                if (hit.distance == 0)
                {
                    break;
                }

                float fraction = hit.distance / distance;
                // Set the fraction of remaining movement (minus some small value)
                position += remaining * (fraction);
                // Push slightly along normal to stop from getting caught in walls
                position += hit.normal * 0.001f * 2;
                // Decrease remaining movement by fraction of movement remaining
                remaining *= (1 - fraction);

                // Plane to project rest of movement onto
                Vector3 planeNormal = hit.normal;

                // Only apply angular change if hitting something
                // Get angle between surface normal and remaining movement
                float angleBetween = Vector3.Angle(hit.normal, remaining) - 90.0f;

                // Normalize angle between to be between 0 and 1
                // 0 means no angle, 1 means 90 degree angle
                angleBetween = Mathf.Min(90f, Mathf.Abs(angleBetween));
                float normalizedAngle = angleBetween / 90f;

                // Reduce the remaining movement by the remaining movement that ocurred
                remaining *= Mathf.Pow(1 - normalizedAngle, anglePower) * 0.9f + 0.1f;

                // Rotate the remaining movement to be projected along the plane 
                // of the surface hit (emulate pushing against the object)
                Vector3 projected = Vector3.ProjectOnPlane(remaining, planeNormal).normalized * remaining.magnitude;

                // If projected remaining movement is less than original remaining movement (so if the projection broke
                // due to float operations), then change this to just project along the vertical.
                if (projected.magnitude + 0.001f < remaining.magnitude)
                {
                    remaining = Vector3.ProjectOnPlane(remaining, Vector3.up).normalized * remaining.magnitude;
                }
                else
                {
                    remaining = projected;
                }

                // Track number of times the character has bounced
                bounces++;
            }

            // We're done, player was moved as part of loop
            return position;
        }

        public (bool, float) CheckGrounded(out RaycastHit groundHit)
        {
            bool onGround = CastSelf(transform.position, transform.rotation, Vector3.down, groundDistance, groundCapsule, out groundHit);
            float angle = Vector3.Angle(groundHit.normal, Vector3.up);
            return (onGround, angle);
        }



        bool CastSelf(Vector3 pos, Quaternion rot, Vector3 dir, float dist, CapsuleCollider capsuleCollider, out RaycastHit hit)
        {
            // Get Parameters associated with the KCC
            Vector3 center = rot * capsuleCollider.center + pos;
            float radius = capsuleCollider.radius;
            float height = capsuleCollider.height;

            Vector3 axisVector = Vector3.up;
            switch (capsuleCollider.direction)
            {
                case 0: axisVector = Vector3.right; break;
                case 1: axisVector = Vector3.up; break;
                case 2: axisVector = Vector3.forward; break;

            };
            // Get top and bottom points of collider
            Vector3 bottom = center + rot * (-axisVector) * (height / 2 - radius);
            Vector3 top = center + rot * axisVector * (height / 2 - radius);


            // Check what objects this collider will hit when cast with this configuration excluding itself
            IEnumerable<RaycastHit> hits = Physics.CapsuleCastAll(
                top, bottom, radius, dir, dist, ~0, QueryTriggerInteraction.Ignore)
                .Where(hit => hit.collider.transform != transform);
            bool didHit = hits.Count() > 0;

            // Find the closest objects hit
            float closestDist = didHit ? Enumerable.Min(hits.Select(hit => hit.distance)) : 0;
            IEnumerable<RaycastHit> closestHit = hits.Where(hit => hit.distance == closestDist);

            // Get the first hit object out of the things the player collides with
            hit = closestHit.FirstOrDefault();

            // Return if any objects were hit
            return didHit;
        }
        
        

        private void OnEnable()
        {
            playerInput.Player.Enable();
        }

        private void OnDisable()
        {
            playerInput.Player.Disable();
        }
    }
}
