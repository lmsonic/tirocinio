using UnityEngine;

namespace Tirocinio
{
    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        {
            isRootState = true;
            ResetXRotation();
            InitializeSubState();
            ctx.Mover.SetKeepOnGround(true);
            ctx.Velocity.y = ctx.GroundedGravity;
        }
        public override void UpdateState()
        {
            SteepSlopeMovement();
            CheckSwitchStates();
        }



        void SteepSlopeMovement()
        {
            if (ctx.OnSteepSlope())
            {
                Vector3 groundNormal = ctx.Mover.GetGroundNormal();
                float groundAngle = ctx.Mover.GetGroundAngle();
                Vector3 groundPoint = ctx.Mover.GetGroundPoint();

                Vector3 slopeDirection = Vector3.up - groundNormal * Vector3.Dot(Vector3.up, groundNormal);
                float slideSpeed = ctx.steepSlopeForce * Time.fixedDeltaTime;
                Vector3 slopeVector = -slopeDirection * slideSpeed;
                ctx.Velocity += slopeVector;
            }
        }

        

        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.IsJumpPressed && !ctx.RequireNewJumpPress)
                SwitchState(factory.Jump());
            else if (!ctx.Mover.IsGrounded())
                SwitchState(factory.Air());

        }
        public override void InitializeSubState()
        {

            if (ctx.BrakeInput > 0.1f)
                SetSubState(factory.Brake());
            else if (ctx.AccelerationInput > 0.1f)
                SetSubState(factory.Acceleration());
            else if (ctx.Velocity.magnitude > 1f)
                SetSubState(factory.Drag());
            else
                SetSubState(factory.Idle());


        }

        void ResetXRotation()
        {
            Vector3 eulers = ctx.transform.localEulerAngles;
            eulers.x = 0f;
            ctx.transform.localEulerAngles = eulers;
        }
    }

    public class PlayerAccelerationState : PlayerBaseState
    {
        public PlayerAccelerationState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState() { }
        public override void UpdateState()
        {

            CheckSwitchStates();

            ctx.LerpGroundedVelocity(ctx.transform.forward * ctx.MaxSpeed,
                ctx.AccelerationMultiplier * ctx.AccelerationInput * Time.fixedDeltaTime);

        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.BrakeInput > 0.1f)
                SwitchState(factory.Brake());
            else if (ctx.AccelerationInput < 0.1f)
                SwitchState(factory.Drag());

        }
        public override void InitializeSubState() { }
    }

    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        {
            ctx.Velocity = Vector3.zero;

        }
        public override void UpdateState()
        {
            CheckSwitchStates();

        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.AccelerationInput > 0.1f)
                SwitchState(factory.Acceleration());
            else if (ctx.CurrentMovement.z < -0.1f)
                SwitchState(factory.Backwards());
            else if (ctx.Velocity.magnitude > 1f)
                SwitchState(factory.Drag());

        }
        public override void InitializeSubState() { }
    }

    public class PlayerBrakeState : PlayerBaseState
    {
        public PlayerBrakeState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState() { }
        public override void UpdateState()
        {

            CheckSwitchStates();

            ctx.LerpGroundedVelocity(Vector3.zero, ctx.BrakeMultiplier * ctx.BrakeInput * Time.fixedDeltaTime);

        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.BrakeInput < 0.1f)
            {
                
                if (ctx.Velocity.magnitude > 1f)
                    SwitchState(factory.Drag());
                else
                    SwitchState(factory.Idle());
            }
        }
        public override void InitializeSubState() { }
    }

    public class PlayerDragState : PlayerBaseState
    {
        public PlayerDragState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState() { }
        public override void UpdateState()
        {

            CheckSwitchStates();


            ctx.LerpGroundedVelocity(Vector3.zero, ctx.DragMultiplier * Time.fixedDeltaTime);


        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {


            if (ctx.BrakeInput > 0.1f)
                SwitchState(factory.Brake());
            else if (ctx.AccelerationInput > 0.1f)
                SwitchState(factory.Acceleration());
            else if (ctx.Velocity.magnitude < 1f)
                SwitchState(factory.Idle());

        }
        public override void InitializeSubState() { }
    }

    public class PlayerBackwardsState : PlayerBaseState
    {
        public PlayerBackwardsState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState() { }
        public override void UpdateState()
        {

            CheckSwitchStates();


            ctx.LerpGroundedVelocity(ctx.transform.forward * ctx.BackwardsSpeed * ctx.CurrentMovement.z, Time.fixedDeltaTime);


        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.CurrentMovement.z > -0.1f)
            {
                SwitchState(factory.Idle());
            }

        }
        public override void InitializeSubState() { }
    }


}