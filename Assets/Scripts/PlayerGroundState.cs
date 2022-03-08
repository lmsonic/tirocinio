using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class PlayerGroundState : PlayerBaseState
    {
        public override void Enter(Player player)
        {

        }
        public override void Process(Player player)
        {

        }
        public override void PhysicsProcess(Player player)
        {

            Vector3 movement = player.GetInputMovement();

            player.Velocity.y = 0f;

            Quaternion rotation = Quaternion.Euler(0f, movement.x, 0f);
            player.FrontTransform.rotation = Quaternion.Lerp(player.FrontTransform.rotation, rotation, 0.1f);

            player.Velocity = Vector3.Lerp(player.Velocity,
                        player.FrontTransform.forward * player.MaxSpeed, player.AccelerationInput * player.Acceleration * Time.deltaTime);

            CharacterController controller = player.GetComponent<CharacterController>();

            controller.Move(player.Velocity * Time.deltaTime);


        }

        void Rotation(Player player)
        {
            //Rotates the handle and body of the scooter at different speeds
            Vector3 movement = player.GetInputMovement();
            Quaternion rotation = Quaternion.Euler(0f, movement.x, 0f);

            player.FrontTransform.rotation = Quaternion.Lerp(player.FrontTransform.rotation, rotation, 0.1f);


        }







        public override void Exit(Player player)
        {

        }
    }
}
