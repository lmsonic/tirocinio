
using UnityEngine;
namespace Tirocinio
{
    public abstract class PlayerBaseState
    {
        protected bool isRootState = false;
        protected PlayerStateMachine ctx;
        protected PlayerStateFactory factory;

        protected PlayerBaseState superState;
        protected PlayerBaseState subState;
        public PlayerBaseState(PlayerStateMachine context, PlayerStateFactory factory)
        {
            this.ctx = context;
            this.factory = factory;
            
        }
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitializeSubState();

        public void UpdateStates() { 
            UpdateState();
            
            if (subState!=null)
                subState.UpdateState();
        }
        protected void SwitchState(PlayerBaseState newState)
        {
            
            Debug.Log(GetType().Name + ":" + subState?.GetType().Name + "->" + newState.GetType().Name + ":" + newState.subState?.GetType().Name);
            ExitState();
            newState.EnterState();
            if (isRootState)
                ctx.CurrentState = newState;
            else if (superState != null)
                superState.SetSubState(newState);
            
        }
        protected void SetSuperState(PlayerBaseState newSuperState) { 
            superState = newSuperState;
        }
        protected void SetSubState( PlayerBaseState newSubState) { 
            subState = newSubState;
            subState.SetSuperState(this);
        }
    }
}
