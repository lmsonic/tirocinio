using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    public class PlayerStateMachine : MonoBehaviour
    {

        PlayerInput playerInput;
        public Mover Mover { get => mover; }
        Mover mover;

        Transform cameraTransform;
        public Transform handleTransform;
        public Transform bodyTransform;
        public float maxTurnDegrees = 30f;
        public float rotationSpeed = 30f;
        public float rotationLerpSpeed = 30f;
        public float MaxSpeed = 30f;
        public float BackwardsSpeed = 10f;

        public float GroundedGravity { get => groundedGravity; }
        float groundedGravity = 0f;
        public float Gravity { get => gravity; }
        float gravity;

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

        public float groundRotationMultiplier = 3f;

        public float aliveAngleLimit = 30f;




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
            mover = GetComponent<Mover>();
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
            //handleTransform.localRotation = FrontRotation();
            //turn
            //bodyTransform.localRotation = BodyRotation();

            Vector3 normal = Vector3.up;
            if (mover.IsGrounded())

                normal = mover.GetGroundNormal();


            Quaternion targetRotationY = Quaternion.AngleAxis(targetAngle - transform.eulerAngles.y, normal);


            Quaternion targetRotationGround = Quaternion.FromToRotation(transform.up, normal);

            Quaternion finalRotation = targetRotationY * targetRotationGround * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationLerpSpeed * Time.deltaTime);



        }

        public void ResetJumpIn(float seconds)
        {
            Invoke("ResetJump", seconds);
        }

        void ResetJump()
        {
            Mover.SetJumping(false);
        }

        Quaternion FrontRotation()
        {
            Quaternion targetRotation = Quaternion.Euler(0f, CurrentMovement.x * maxTurnDegrees, 0f);
            return Quaternion.Slerp(handleTransform.localRotation, targetRotation, 2f * Time.deltaTime);
        }

        Quaternion BodyRotation()
        {
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -CurrentMovement.x * maxTurnDegrees);
            return Quaternion.Slerp(bodyTransform.localRotation, targetRotation, Time.deltaTime);
        }




        private void LateUpdate()
        {
            HandleRotation();
        }

        public void LerpGroundedVelocity(Vector3 target, float t)
        {

            Velocity = Vector3.Lerp(Velocity, target, t);

        }



        private void FixedUpdate()
        {

            mover.CheckForGround();
            currentState.UpdateStates();

            mover.SetExtendSensorRange(mover.IsGrounded());

            mover.SetVelocity(Velocity);
        }





        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Velocity);
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
