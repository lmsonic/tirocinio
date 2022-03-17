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

        }
        public override void UpdateState()
        {
            CheckSwitchStates();
            if (ctx.IsJumpPressed)
                ctx.SnapPlayerDown();
            

        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            (bool onGround, float groundAngle) = ctx.CheckGrounded(out RaycastHit groundHit);
            bool falling = !(onGround && groundAngle <= ctx.maxWalkingAngle);

            if (ctx.IsJumpPressed && !ctx.RequireNewJumpPress)
                SwitchState(factory.Jump());
            else if (falling)
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

        void ResetXRotation(){
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
            ctx.Velocity = Vector3.Lerp(ctx.Velocity, ctx.transform.forward * ctx.MaxSpeed, ctx.AccelerationMultiplier * ctx.AccelerationInput * Time.deltaTime);


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
            ctx.Velocity = Vector3.Lerp(ctx.Velocity, Vector3.zero, ctx.BrakeMultiplier * ctx.BrakeInput * Time.deltaTime);

        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.BrakeInput < 0.1f)
            {
                Vector3 groundedVelocity = ctx.Velocity;
                groundedVelocity.y = 0f;

                if (groundedVelocity.magnitude > 1f)
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

            ctx.Velocity = Vector3.Lerp(ctx.Velocity, Vector3.zero, ctx.DragMultiplier * Time.deltaTime);

        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            Vector3 groundedVelocity = ctx.Velocity;
            groundedVelocity.y = 0f;

            if (ctx.BrakeInput > 0.1f)
                SwitchState(factory.Brake());
            else if (ctx.AccelerationInput > 0.1f)
                SwitchState(factory.Acceleration());
            else if (groundedVelocity.magnitude < 1f)
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

            ctx.Velocity = Vector3.Lerp(ctx.Velocity, ctx.transform.forward * ctx.BackwardsSpeed * ctx.CurrentMovement.z, Time.deltaTime);


        }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            if (ctx.CurrentMovement.z > - 0.1f)
            {
                SwitchState(factory.Idle());
            }

        }
        public override void InitializeSubState() { }
    }


}