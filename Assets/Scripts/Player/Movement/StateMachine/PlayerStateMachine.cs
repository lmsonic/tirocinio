using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Mover))]
    public class PlayerStateMachine : MonoBehaviour
    {
        public PlayerInput PlayerInput { get => playerInput; }
        PlayerInput playerInput;
        public Mover Mover { get => mover; }
        Mover mover;

        Transform cameraTransform;
        [Header("Transform References")]
        public Transform handleTransform;
        public Transform bodyTransform;
        public Transform modelTransform;

        [Header("Rotation Variables")]
        public float maxTurnDegrees = 30f;
        public float rotationSpeed = 30f;
        public float rotationLerpSpeed = 30f;
        public float returnToNormalLerpSpeed = 10f;

        [Header("Ground Movement Variables")]
        public float MaxSpeed = 30f;
        public float BackwardsSpeed = 10f;

        public float GroundedGravity { get => groundedGravity; }
        float groundedGravity = 0f;
        public float Gravity { get => gravity; }
        float gravity;

        public Vector3 Velocity;

        public float AccelerationMultiplier = 2f;
        public float BrakeMultiplier = 3f;
        public float DragMultiplier = 1.5f;


        float initialJumpVelocity;

        public float InitialJumpVelocity { get => initialJumpVelocity; }

        [Header("Air Movement Variables")]
        public float maxJumpHeight = 1f;
        public float maxJumpTime = 0.5f;
        public float MaxFallSpeed = -30f;
        public float FallMultiplier = 1.5f;

        [Header("Slope Variables")]

        [Range(0, 90f)]
        public float steepSlopeLimit = 45f;
        public float steepSlopeForce = 15f;




        public PlayerBaseState CurrentState
        {
            set { currentState = value; }
            get { return currentState; }
        }

        PlayerBaseState currentState;

        PlayerStateFactory states;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            mover = GetComponent<Mover>();
            cameraTransform = Camera.main.transform;

            states = new PlayerStateFactory(this);

            currentState = states.Grounded();
            currentState.EnterState();

            SetupJumpVariables();
        }

        void SetupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = 2 * maxJumpHeight / timeToApex;
        }





        void HandleRotation()
        {
            float targetAngle = cameraTransform.eulerAngles.y;

            //rotate handle
            Quaternion targetRotation = Quaternion.Euler(0f, playerInput.CurrentMovement.x * maxTurnDegrees, 0f);
            handleTransform.localRotation = Quaternion.Slerp(handleTransform.localRotation, targetRotation, 2f * Time.deltaTime);
            //turn
            targetRotation = Quaternion.Euler(-90f, 0f, -playerInput.CurrentMovement.x * maxTurnDegrees);
            bodyTransform.localRotation = Quaternion.Slerp(bodyTransform.localRotation, targetRotation, Time.deltaTime);

            targetRotation = Quaternion.Euler(0f, 0f, -playerInput.CurrentMovement.x * maxTurnDegrees);
            modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, targetRotation, Time.deltaTime);

            Vector3 normal = Vector3.Lerp(transform.up, Vector3.up, returnToNormalLerpSpeed);

            if (mover.IsGrounded())
            {
                Vector3 groundNormal = mover.GetGroundNormal();
                normal = groundNormal;
            }

            Quaternion targetRotationY = Quaternion.AngleAxis(targetAngle - transform.eulerAngles.y, normal);

            Quaternion targetRotationGround = Quaternion.FromToRotation(transform.up, normal);

            Quaternion finalRotation = targetRotationY * targetRotationGround * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationLerpSpeed * Time.deltaTime);



        }


        public void DisableKeepOnGroundFor(float seconds)
        {
            mover.SetKeepOnGround(false);
            Invoke("ResetKeepOnGround", seconds);
        }

        void ResetKeepOnGround()
        {
            Mover.SetKeepOnGround(true);
        }


        private void Update()
        {
            HandleRotation();
        }

        public void LerpGroundedVelocity(Vector3 target, float t)
        {
            Vector3 groundedDirection = target;
            groundedDirection.y = 0f;
            Velocity = Vector3.Lerp(Velocity, groundedDirection, t);
        }



        private void FixedUpdate()
        {

            mover.CheckForGround();
            Velocity = mover.GetVelocity();
            currentState.UpdateStates();
            mover.SetVelocity(Velocity);
        }

        public bool OnSteepSlope()
        {
            if (!mover.IsGrounded()) return false;

            float angle = Vector3.Angle(Vector3.up, mover.GetGroundNormal());
            if (angle > steepSlopeLimit)
                return true;

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Velocity);
        }
    }
}
