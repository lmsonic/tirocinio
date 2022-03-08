using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public abstract class PlayerBaseState
    {
        // Start is called before the first frame update
        public abstract void Enter(Player player);
        public abstract void Process(Player player);
        public abstract void PhysicsProcess(Player player);
        public abstract void Exit(Player player);
    }
}
