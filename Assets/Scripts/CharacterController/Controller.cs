using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    [RequireComponent(typeof(Mover))]
    public class Controller : MonoBehaviour
    {
        private Mover mover;
        private void Awake() {
            mover= GetComponent<Mover>();
        }

        public InputActionReference movementInput;

        private void OnEnable() {
            movementInput.action.Enable();
        }

        private void OnDisable() {
            movementInput.action.Disable();
        }

        private void FixedUpdate() {
            mover.CheckForGround();
            bool isGrounded = mover.IsGrounded();

            Vector3 velocity = mover.GetVelocity();
            Vector2 movement = movementInput.action.ReadValue<Vector2>();

            if (isGrounded){
                velocity.x = movement.x * 10f;
                velocity.z = movement.y * 10f;

            }
            else
                velocity.y -= 10f * Time.fixedDeltaTime;

            mover.SetExtendSensorRange(isGrounded);
            mover.SetVelocity(velocity);
        }

    }
}