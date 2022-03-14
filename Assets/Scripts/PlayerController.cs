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
        Vector2 currentMovementInput;
        Vector3 currentMovement;
        float accelerationInput;
        Vector3 groundVelocity;



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
        }

        void OnAccelerationInput(InputAction.CallbackContext context)
        {
            accelerationInput = context.ReadValue<float>();
        }



        void OnMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            currentMovement.y = 0f;

        }


        void HandleRotation()
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            //rotate handle
            Quaternion targetRotation = Quaternion.Euler(0f, currentMovement.x * maxTurnDegrees, 0f);
            handleTransform.localRotation = Quaternion.Slerp(handleTransform.localRotation, targetRotation,  Time.deltaTime);
            //turn
            targetRotation = Quaternion.Euler(0f, 0f, -currentMovement.x * maxTurnDegrees);
            bodyTransform.localRotation = Quaternion.Slerp(bodyTransform.localRotation, targetRotation,  Time.deltaTime);

            //actual rotation
            targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);



        }


        void Update()
        {
            HandleRotation();




            groundVelocity = Vector3.Lerp(groundVelocity, transform.forward * maxSpeed, accelerationInput * Time.deltaTime);

            if (accelerationInput < 0.1f)
                groundVelocity *= 0.99f;

            characterController.Move(groundVelocity * Time.deltaTime);

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
