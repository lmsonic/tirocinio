
using UnityEngine;
namespace Tirocinio
{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        {
            isRootState = true;

        }
            
        
        public override void UpdateState()
        {
            CheckSwitchStates();

            

            bool isFalling = ctx.Velocity.y <= 0f || !ctx.IsJumpPressed;

            if (isFalling)
            {
                ctx.Velocity.y = Mathf.Max(ctx.Velocity.y + ctx.Gravity * ctx.FallMultiplier * Time.fixedDeltaTime, ctx.MaxFallSpeed);
            }
            else
            {
                ctx.Velocity.y = ctx.Velocity.y + ctx.Gravity * Time.fixedDeltaTime;
            }

        }
        public override void ExitState()
        {
            if (ctx.IsJumpPressed)
                ctx.RequireNewJumpPress = true;

        }
        public override void CheckSwitchStates()
        {
            if (ctx.Mover.IsGrounded())
                SwitchState(factory.Grounded());
                
        }

        public override void InitializeSubState() { }
    }
}