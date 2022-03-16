using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    public class PlayerController : MonoBehaviour
    {
        PlayerInput playerInput;
        CharacterController characterController;
        Animator animator;
        Transform cameraTransform;
        public Transform handleTransform;
        public Transform bodyTransform;
        public float maxTurnDegrees = 30f;
        public float rotationSpeed = 30f;
        public float maxSpeed = 30f;

        float groundedGravity = -1f;
        float gravity = -9.8f;

        Vector2 currentMovementInput;
        Vector3 currentMovement;
        float accelerationInput;
        float brakeInput;
        Vector3 velocity;

        public float accelerationMultiplier = 2f;
        public float brakeMultiplier = 3f;
        public float dragMultiplier = 1.5f;

        bool isJumpPressed = false;
        float initialJumpVelocity;
        public float maxJumpHeight = 1f;
        public float maxJumpTime = 0.5f;
        public float maxFallSpeed = -30f;
        public float fallMultiplier = 1.5f;
        bool isJumping = false;


        private void Awake()
        {
            playerInput = new PlayerInput();
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;

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
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            currentMovement.y = 0f;

        }

        void HandleJump()
        {

            if (!isJumping && characterController.isGrounded && isJumpPressed)
            {
                isJumping = true;

                velocity.y = initialJumpVelocity;
            }
            else if (!isJumpPressed && isJumping && characterController.isGrounded)
            {
                isJumping = false;
            }


        }




        void HandleRotation()
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            //rotate handle
            Quaternion targetRotation = Quaternion.Euler(0f, currentMovement.x * maxTurnDegrees, 0f);
            handleTransform.localRotation = Quaternion.Slerp(handleTransform.localRotation, targetRotation, Time.deltaTime);
            //turn
            targetRotation = Quaternion.Euler(0f, 0f, -currentMovement.x * maxTurnDegrees);
            bodyTransform.localRotation = Quaternion.Slerp(bodyTransform.localRotation, targetRotation, Time.deltaTime);

            //actual rotation
            targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        }

        void HandleGravity()
        {
            bool isFalling = velocity.y <= 0f || !isJumpPressed;


            if (characterController.isGrounded)
            {
                velocity.y = groundedGravity;
            }
            else if (isFalling)
            {
                velocity.y = Mathf.Max(velocity.y + gravity * fallMultiplier * Time.deltaTime, maxFallSpeed);
            }
            else
            {
                velocity.y = velocity.y + gravity * Time.deltaTime;
            }
        }


        void Update()
        {
            HandleRotation();
            float yVelocityBefore = velocity.y;
            if (!isJumping)
            {
                if (brakeInput > 0.1f)//brake
                {
                    velocity = Vector3.Lerp(velocity, Vector3.zero, brakeMultiplier * brakeInput * Time.deltaTime);
                }
                else if (accelerationInput > 0.1f)//acceleration
                {
                    velocity = Vector3.Lerp(velocity, transform.forward * maxSpeed, accelerationMultiplier * accelerationInput * Time.deltaTime);
                }
                else if (accelerationInput < 0.1f)//drag
                {
                    velocity = Vector3.Lerp(velocity, Vector3.zero, dragMultiplier * Time.deltaTime);
                }
            }



            velocity.y = yVelocityBefore;

            characterController.Move(velocity * Time.deltaTime);
            HandleGravity();
            HandleJump();

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
