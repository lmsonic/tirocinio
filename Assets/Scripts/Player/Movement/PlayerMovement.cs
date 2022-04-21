using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace Tirocinio
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Mover))]
    public class PlayerMovement : MonoBehaviour
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



        HybridStateMachine fsm;
        private void Start()
        {
            HybridStateMachine groundState = InitializeGroundStateMachine();
            fsm = new HybridStateMachine();
            fsm.AddState("Ground", groundState);

            fsm.AddState("Air", onLogic: (state) =>
            {
                bool isFalling = Velocity.y <= 0f || !PlayerInput.IsJumpPressed;

                if (isFalling)
                {
                    Velocity.y = Mathf.Max(Velocity.y + Gravity * FallMultiplier * Time.fixedDeltaTime, MaxFallSpeed);
                }
                else
                {
                    Velocity.y = Velocity.y + Gravity * Time.fixedDeltaTime;
                }
            },
            onExit: (state) =>
            {
                if (PlayerInput.IsJumpPressed)
                    PlayerInput.RequireNewJumpPress = true;
            });


            fsm.AddState("Jump",
                onEnter: (state) => Velocity.y = InitialJumpVelocity,
                onLogic: (state) => fsm.RequestStateChange("Air"));

            fsm.AddTransition("Ground", "Jump",
                (transition) => PlayerInput.IsJumpPressed && !PlayerInput.RequireNewJumpPress);

            fsm.AddTransition("Ground", "Air", (transition) => !Mover.IsGrounded());
            fsm.AddTransition("Air", "Ground", (transition) => Mover.IsGrounded());

            fsm.SetStartState("Ground");

            fsm.Init();

        }

        HybridStateMachine InitializeGroundStateMachine()
        {
            HybridStateMachine groundFSM = new HybridStateMachine(
                onEnter: (state) =>
                {
                    ResetXRotation();
                    Mover.SetKeepOnGround(true);
                    Velocity.y = 0f;
                },
                onLogic: (state) =>
                {
                    SteepSlopeMovement();
                }
            );

            groundFSM.AddState("Idle", onEnter: (state) => Velocity = Vector3.zero);

            groundFSM.AddState("Acceleration", onLogic: (state) =>
            {
                LerpGroundedVelocity(transform.forward * MaxSpeed,
                                AccelerationMultiplier * PlayerInput.AccelerationInput * Time.fixedDeltaTime);
            });

            groundFSM.AddState("Brake", onLogic: (state) =>
            {
                LerpGroundedVelocity(Vector3.zero,
                                BrakeMultiplier * PlayerInput.BrakeInput * Time.fixedDeltaTime);
            });

            groundFSM.AddState("Drag", onLogic: (state) =>
            {
                LerpGroundedVelocity(Vector3.zero,
                                DragMultiplier * Time.fixedDeltaTime);
            });

            groundFSM.AddState("Backwards", onLogic: (state) =>
            {
                LerpGroundedVelocity(transform.forward * BackwardsSpeed * PlayerInput.CurrentMovement.z,
                                Time.fixedDeltaTime);
            });

            groundFSM.AddTransition("Idle", "Acceleration",
                (transition) => PlayerInput.AccelerationInput > 0.1f
            );
            groundFSM.AddTransition("Idle", "Backwards",
                (transition) => PlayerInput.CurrentMovement.z < -0.1f
            );
            groundFSM.AddTransition("Idle", "Drag",
                (transition) => Velocity.magnitude > 1f
            );

            groundFSM.AddTransition("Acceleration", "Brake",
                (transition) => PlayerInput.BrakeInput > 0.1f
            );
            groundFSM.AddTransition("Acceleration", "Drag",
                (transition) => PlayerInput.AccelerationInput < 0.1f
            );

            groundFSM.AddTransition("Brake", "Drag",
                (transition) => PlayerInput.BrakeInput < 0.1f && Velocity.magnitude > 1f
            );
            groundFSM.AddTransition("Brake", "Idle",
                (transition) => PlayerInput.BrakeInput < 0.1f && Velocity.magnitude <= 1f
            );

            groundFSM.AddTransition("Drag", "Acceleration",
                (transition) => PlayerInput.AccelerationInput > 0.1f
            );
            groundFSM.AddTransition("Drag", "Brake",
                (transition) => PlayerInput.BrakeInput > 0.1f
            );
            groundFSM.AddTransition("Drag", "Idle",
                (transition) => Velocity.magnitude < 1f
            );

            groundFSM.AddTransition("Backwards", "Idle",
                (transition) => PlayerInput.CurrentMovement.z > -0.1f
            );

            groundFSM.SetStartState("Idle");

            groundFSM.Init();

            return groundFSM;

        }
        void SteepSlopeMovement()
        {
            if (OnSteepSlope())
            {
                Vector3 groundNormal = Mover.GetGroundNormal();
                float groundAngle = Mover.GetGroundAngle();
                Vector3 groundPoint = Mover.GetGroundPoint();

                Vector3 slopeDirection = Vector3.up - groundNormal * Vector3.Dot(Vector3.up, groundNormal);
                float slideSpeed = steepSlopeForce * Time.fixedDeltaTime;
                Vector3 slopeVector = -slopeDirection * slideSpeed;
                Velocity += slopeVector;
            }
        }

        void EnterGroundState()
        {
            ResetXRotation();
            Mover.SetKeepOnGround(true);
            Velocity.y = 0f;
        }


        void ResetXRotation()
        {
            Vector3 eulers = transform.localEulerAngles;
            eulers.x = 0f;
            transform.localEulerAngles = eulers;
        }

        private void Awake()
        {

            playerInput = GetComponent<PlayerInput>();
            mover = GetComponent<Mover>();
            cameraTransform = Camera.main.transform;

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
            fsm.OnLogic();
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
