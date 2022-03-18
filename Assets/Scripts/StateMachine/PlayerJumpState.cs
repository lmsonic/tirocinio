

namespace Tirocinio
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        {
            isRootState = true;
            ctx.Velocity.y = ctx.InitialJumpVelocity;
            ctx.groundChecker.SetGroundedFalseFor(0.5f);

        }
        public override void UpdateState()
        {
            SwitchState(factory.Air());
        }
        public override void ExitState() { }
        public override void CheckSwitchStates() { }
        public override void InitializeSubState() { }

    }
}