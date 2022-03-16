using System.Collections;
using System.Collections.Generic;

namespace Tirocinio
{
    public class PlayerStateFactory
    {
        PlayerStateMachine context;

        enum States{
            GROUNDED,
            IDLE,
            BACKWARDS,  
            ACCELERATION,
            BRAKE,
            DRAG,
            JUMP,
            AIR,
        }
        public PlayerStateFactory(PlayerStateMachine context)
        {
            this.context = context;
            states[States.GROUNDED] = new PlayerGroundedState(context, this);
            states[States.IDLE] = new PlayerIdleState(context, this);
            states[States.BACKWARDS] = new PlayerBackwardsState(context, this);
            states[States.ACCELERATION] = new PlayerAccelerationState(context, this);
            states[States.BRAKE] = new PlayerBrakeState(context, this);
            states[States.DRAG] = new PlayerDragState(context, this);
            states[States.JUMP] = new PlayerJumpState(context, this);
            states[States.AIR] = new PlayerAirState(context, this);
        }

        Dictionary<States,PlayerBaseState> states = new Dictionary<States, PlayerBaseState>();

        public PlayerBaseState Idle() =>  states[States.IDLE];
        public PlayerBaseState Backwards() => states[States.BACKWARDS];
        public PlayerBaseState Acceleration() =>  states[States.ACCELERATION];
        public PlayerBaseState Brake() =>  states[States.BRAKE];
        public PlayerBaseState Drag() =>  states[States.DRAG];
        public PlayerBaseState Jump() =>  states[States.JUMP];
        public PlayerBaseState Air() => states[States.AIR];
        public PlayerBaseState Grounded() => states[States.GROUNDED];
    }
}