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
        PlayerInput playerInput;

        Mover mover;

        Transform cameraTransform;
        [Header("Transform References")]
        public Transform handleTransform;
        public Transform bodyTransform;
        public Transform modelTransform;

        [Header("Rotation Variables")]
        public float maxTurnDegrees = 30f;
        public float rotationLerpSpeed = 30f;
        public float returnToNormalLerpSpeed = 10f;

        [Header("Ground Movement Variables")]
        public float MaxSpeed = 30f;
        public float BackwardsSpeed = 10f;
        public float GrindSpeed = 10f;


        float gravity;

        public Vector3 velocity = Vector3.zero;

        public float AccelerationMultiplier = 2f;
        public float BrakeMultiplier = 3f;
        public float DragMultiplier = 1.5f;


        float initialJumpVelocity;


        [Header("Air Movement Variables")]
        public float maxJumpHeight = 1f;
        public float maxJumpTime = 0.5f;
        public float MaxFallSpeed = -30f;
        public float FallMultiplier = 1.5f;
        public float AirSpeed = 10f;

        [Header("Slope Variables")]

        [Range(0, 90f)]
        public float steepSlopeLimit = 45f;
        public float slideSpeed = 15f;



        HybridStateMachine fsm;

        BezierSpline grindingSpline = null;
        float grindJumpTimer = 1f;
        const float grindJumpTime = 0.5f;




        public void SetGrindingState(BezierSpline spline)
        {
            if (grindJumpTimer >= grindJumpTime && velocity.y < -0.5f)
            {
                Debug.Log("Set Spline with velocity " + velocity);
                grindingSpline = spline;
            }
        }

        private void Start()
        {
            HybridStateMachine groundState = InitializeGroundStateMachine();
            fsm = new HybridStateMachine();
            fsm.AddState("Ground", groundState);

            fsm.AddState("Air", onEnter: (state) => Debug.Log(state.name),
             onLogic: (state) =>
            {
                bool isFalling = velocity.y <= 0f || !playerInput.IsJumpPressed;

                Vector3 movement = playerInput.CurrentMovement.x * transform.right + playerInput.CurrentMovement.z * transform.forward;
                velocity += movement * AirSpeed * Time.fixedDeltaTime;

                velocity.y = isFalling ?
                            Mathf.Max(velocity.y + gravity * FallMultiplier * Time.fixedDeltaTime, MaxFallSpeed) :
                            velocity.y + gravity * Time.fixedDeltaTime;

            },
            onExit: (state) =>
            {
                if (playerInput.IsJumpPressed)
                    playerInput.RequireNewJumpPress = true;
            });


            fsm.AddState("Jump",
                onEnter: (state) =>
                {
                    Debug.Log(state.name);
                    velocity.y = initialJumpVelocity;
                },
                onLogic: (state) => fsm.RequestStateChange("Air"));

            fsm.AddState("Grinding",
            onEnter: (state) =>
            {
                Debug.Log(state.name);
                grindJumpTimer = 0f;
                float t;
                (transform.position, t) = grindingSpline.GetClosestPoint(transform.position);

                velocity = GetGrindVelocity(t);
            },
            onLogic: (state) =>
            {
                float t; Vector3 closestPosition;
                (closestPosition, t) = grindingSpline.GetClosestPoint(transform.position);

                Vector3 delta = closestPosition - transform.position;
                velocity = delta + GetGrindVelocity(t);

                if (velocity.magnitude < 0.5f) fsm.RequestStateChange("Air");
                if ((t > 1f || t < 0f) && grindingSpline != null && !grindingSpline.Loop) fsm.RequestStateChange("Air");
            },
            onExit: (state) => grindingSpline = null);

            fsm.AddTransition("Ground", "Jump",
                (transition) => playerInput.IsJumpPressed && !playerInput.RequireNewJumpPress);

            fsm.AddTransition("Ground", "Air", (transition) => !mover.IsGrounded());
            fsm.AddTransition("Air", "Ground", (transition) => mover.IsGrounded());
            fsm.AddTransition("Air", "Grinding", (transition) => grindingSpline != null);
            fsm.AddTransition("Grinding", "Jump", (transition) => playerInput.IsJumpPressed && !playerInput.RequireNewJumpPress);

            fsm.SetStartState("Ground");

            fsm.Init();

        }
        const float minGrindVelocity = 5f;

        Vector3 GetGrindVelocity(float t)
        {
            Vector3 grindDirection = grindingSpline.GetDirection(t);
            grindDirection = Vector3.Dot(velocity.normalized, grindDirection) * grindDirection;

            Vector3 right = Quaternion.Euler(0f, 90f, 0f) * grindDirection;


            Vector3 grindDirectionXZ = new Vector3(grindDirection.x, 0f, grindDirection.z);

            float angle = Vector3.SignedAngle(grindDirectionXZ, grindDirection, right);
            Debug.Log(angle);

            float steepMultiplier = Mathf.InverseLerp(-90, 90, angle);
            Debug.Log(steepMultiplier);


            Vector3 optimalGrindDirectionPos = Quaternion.Euler(0f, 40f, 0f) * grindDirection;
            Vector3 optimalGrindDirectionNeg = Quaternion.Euler(0f, -40f, 0f) * grindDirection;

            Vector3 optimalGrindDirection =
            (transform.forward - optimalGrindDirectionPos).sqrMagnitude < (transform.forward - optimalGrindDirectionNeg).sqrMagnitude
                ? optimalGrindDirectionPos : optimalGrindDirectionNeg;


            Vector3 vel = Mathf.Clamp01(Vector3.Dot(transform.forward, optimalGrindDirection)) * grindDirection;
            vel *= GrindSpeed * steepMultiplier;
            if (vel.magnitude < minGrindVelocity)
                vel = vel.normalized * minGrindVelocity;


            return vel;
        }



        HybridStateMachine InitializeGroundStateMachine()
        {
            HybridStateMachine groundFSM = new HybridStateMachine(
                onEnter: (state) =>
                {
                    Debug.Log(state.name);
                    velocity.y = 0f;
                },
                onLogic: (state) =>
                {
                    SteepSlopeMovement();
                }
            );


            groundFSM.AddState("Idle", onEnter: (state) => Debug.Log(state.name),
             onLogic: (state) => velocity = Vector3.zero, needsExitTime: false);

            groundFSM.AddState("Acceleration", onEnter: (state) => Debug.Log(state.name),
            onLogic: (state) =>
            {

                LerpGroundedVelocity(transform.forward * MaxSpeed,
                                AccelerationMultiplier * playerInput.AccelerationInput * Time.fixedDeltaTime);
            });

            groundFSM.AddState("Brake", onEnter: (state) => Debug.Log(state.name),
            onLogic: (state) =>
            {
                LerpGroundedVelocity(Vector3.zero,
                                BrakeMultiplier * playerInput.BrakeInput * Time.fixedDeltaTime);
            });

            groundFSM.AddState("Drag", onEnter: (state) => Debug.Log(state.name),
            onLogic: (state) =>
            {
                LerpGroundedVelocity(Vector3.zero,
                                DragMultiplier * Time.fixedDeltaTime);
            });

            groundFSM.AddState("Backwards", onEnter: (state) => Debug.Log(state.name),
             onLogic: (state) =>
            {
                LerpGroundedVelocity(transform.forward * BackwardsSpeed * playerInput.CurrentMovement.z,
                                Time.fixedDeltaTime);
            });

            groundFSM.AddTransition("Idle", "Acceleration",
                (transition) => playerInput.AccelerationInput > 0.1f
            );
            groundFSM.AddTransition("Idle", "Backwards",
                (transition) => playerInput.CurrentMovement.z < -0.1f
            );
            groundFSM.AddTransition("Idle", "Drag",
                (transition) => velocity.magnitude > 1f
            );

            groundFSM.AddTransition("Acceleration", "Brake",
                (transition) => playerInput.BrakeInput > 0.1f
            );
            groundFSM.AddTransition("Acceleration", "Drag",
                (transition) => playerInput.AccelerationInput < 0.1f
            );

            groundFSM.AddTransition("Brake", "Drag",
                (transition) => playerInput.BrakeInput < 0.1f && velocity.magnitude > 1f
            );
            groundFSM.AddTransition("Brake", "Idle",
                (transition) => playerInput.BrakeInput < 0.1f && velocity.magnitude <= 1f
            );

            groundFSM.AddTransition("Drag", "Acceleration",
                (transition) => playerInput.AccelerationInput > 0.1f
            );
            groundFSM.AddTransition("Drag", "Brake",
                (transition) => playerInput.BrakeInput > 0.1f
            );
            groundFSM.AddTransition("Drag", "Idle",
                (transition) => velocity.magnitude < 1f
            );

            groundFSM.AddTransition("Backwards", "Idle",
                (transition) => playerInput.CurrentMovement.z > -0.1f
            );


            groundFSM.SetStartState("Idle");

            groundFSM.Init();

            return groundFSM;

        }
        void SteepSlopeMovement()
        {
            if (mover.IsGrounded())
            {
                Vector3 groundNormal = mover.GetGroundNormal();
                if (IsOnSteepSlope(groundNormal))
                {
                    Vector3 slideDirection = Vector3.zero;
                    slideDirection.x = ((1f - groundNormal.y) * groundNormal.x) * slideSpeed;
                    slideDirection.z = ((1f - groundNormal.y) * groundNormal.z) * slideSpeed;
                    velocity += slideDirection;
                }

            }
        }

        bool IsOnSteepSlope(Vector3 normal) => Vector3.Angle(Vector3.up, normal) >= steepSlopeLimit;


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

        private void OnValidate()
        {
            SetupJumpVariables();
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

        Quaternion GetVelocityRot()
        {
            Vector3 vel = velocity;
            if (vel.magnitude > 0.2f)
            {
                vel.y = 0;
                Vector3 direction = transform.forward;
                direction.y = 0;
                Quaternion velocityRotation = Quaternion.FromToRotation(direction.normalized, vel.normalized);
                return velocityRotation;
            }
            else
                return Quaternion.identity;
        }

        Quaternion GetGroundRotation()
        {
            Vector3 groundNormal = Vector3.up;
            if (mover.IsGrounded())
                groundNormal = mover.GetGroundNormal();

            return Quaternion.FromToRotation(transform.up, groundNormal);
        }





        private void Update()
        {
            HandleRotation();
        }

        public void LerpGroundedVelocity(Vector3 target, float t)
        {
            target.y = 0f;
            velocity = Vector3.MoveTowards(velocity, target, t);

        }



        private void FixedUpdate()
        {
            mover.CheckForGround();
            velocity = mover.GetVelocity();
            fsm.OnLogic();
            if (fsm.ActiveStateName != "Grinding")
            {
                grindJumpTimer += Time.fixedDeltaTime;
            }
            mover.SetVelocity(velocity);
        }



        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, velocity);
        }
    }
}
