
using UnityEngine;
namespace Tirocinio
{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        float smoothDampVelocity;
        public override void EnterState()
        {
            isRootState = true;

        }
        public override void UpdateState()
        {
            CheckSwitchStates();

            RotateXAxis();


            bool isFalling = ctx.Velocity.y <= 0f || !ctx.IsJumpPressed;

            if (isFalling)
            {
                ctx.Velocity.y = Mathf.Max(ctx.Velocity.y + ctx.Gravity * ctx.FallMultiplier * Time.deltaTime, ctx.MaxFallSpeed);
            }
            else
            {
                ctx.Velocity.y = ctx.Velocity.y + ctx.Gravity * Time.deltaTime;
            }

        }
        public override void ExitState()
        {
            if (ctx.IsJumpPressed)
                ctx.RequireNewJumpPress = true;

        }
        public override void CheckSwitchStates()
        {
            if (ctx.groundChecker.isGrounded)
                SwitchState(factory.Grounded());
                
        }

        public override void InitializeSubState() { }
        void RotateXAxis()
        {
            Vector3 eulers = ctx.transform.localEulerAngles;
            float targetAngle = Mathf.Atan2(ctx.Velocity.y, 1f) * Mathf.Rad2Deg;
            eulers.x = Mathf.SmoothDampAngle(eulers.x, -targetAngle, ref smoothDampVelocity, 0.15f);

            ctx.transform.localEulerAngles = eulers;
        }
    }
}