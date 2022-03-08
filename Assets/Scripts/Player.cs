using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update

        Vector2 _inputMovement = Vector2.zero;
        public Vector3 GetInputMovement() => new Vector3(_inputMovement.x, 0f, _inputMovement.y);
        public void Move(InputAction.CallbackContext ctx) => _inputMovement = ctx.ReadValue<Vector2>();

        public bool IsJumping { get { return _isJumping; } }
        bool _isJumping = false;
        public void Jump(InputAction.CallbackContext ctx) => _isJumping = ctx.performed;

        public float AccelerationInput { get { return _accelerationInput; } }

        float _accelerationInput = 0f;
        public void Accelerate(InputAction.CallbackContext ctx) => _accelerationInput = ctx.ReadValue<float>();

        public float BrakeInput { get { return _brakeInput; } }
        float _brakeInput = 0f;
        public void Brake(InputAction.CallbackContext ctx) => _brakeInput = ctx.ReadValue<float>();

        Rigidbody rb;

        public Transform CameraTransform { get { return _cameraTransform; } }
        Transform _cameraTransform;

        public Transform FrontTransform;
        PlayerBaseState currentState;
        PlayerGroundState groundState = new PlayerGroundState();

        [HideInInspector]
        public Vector3 Velocity = Vector3.zero;

        public float MaxSpeed = 20f;
        public float Acceleration = 20f;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            _cameraTransform = Camera.main.transform;

            ChangeState(groundState);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            currentState.PhysicsProcess(this);
        }

        public void ChangeState(PlayerBaseState state)
        {
            currentState?.Exit(this);
            currentState = state;
            currentState?.Enter(this);
        }
    }
}
