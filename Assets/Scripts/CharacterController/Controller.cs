using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    [RequireComponent(typeof(Mover))]
    public class Controller : MonoBehaviour
    {
        private Mover mover;



        public PlayerInput playerInput;

        public float movementSpeed = 20f;
        public float rotationSpeed = 10f;
        public float gravity = 20f;

        private void OnEnable()
        {
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }

        Vector2 movementInput;
        Vector2 rotationInput;

        private void Awake()
        {
            playerInput = new PlayerInput();
            mover = GetComponent<Mover>();
            playerInput.BasicInput.Move.started += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerInput.BasicInput.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerInput.BasicInput.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerInput.BasicInput.Look.started += ctx => rotationInput = ctx.ReadValue<Vector2>();
            playerInput.BasicInput.Look.performed += ctx => rotationInput = ctx.ReadValue<Vector2>();
            playerInput.BasicInput.Look.canceled += ctx => rotationInput = ctx.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            mover.CheckForGround();
            bool isGrounded = mover.IsGrounded();


            Vector2 rotation = rotationInput;


            Vector3 velocity = transform.forward * movementInput.y + transform.right * movementInput.x;
            velocity = velocity.normalized * movementSpeed;

            if (!isGrounded){
                velocity.y = mover.GetVelocity().y;
                velocity.y -= gravity * Time.fixedDeltaTime;
            }



            Vector3 eulers = transform.eulerAngles;
            eulers.y += rotationInput.x * rotationSpeed;
            transform.eulerAngles = eulers;


            mover.SetExtendSensorRange(isGrounded);
            mover.SetVelocity(velocity);
        }

    }
}