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
        public float jumpVelocity = 20f;
        public float rotationLerpSpeed = 20f;

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
        bool jumpInput;


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

            playerInput.BasicInput.Jump.started += ctx => jumpInput = ctx.ReadValueAsButton();
            playerInput.BasicInput.Jump.canceled += ctx => jumpInput = ctx.ReadValueAsButton();

        }

        private void FixedUpdate()
        {
            mover.CheckForGround();
            bool isGrounded = mover.IsGrounded();




            Vector3 velocity = transform.forward * movementInput.y + transform.right * movementInput.x;
            velocity = velocity.normalized * movementSpeed;

            if (isGrounded && jumpInput)
            {
                velocity.y += jumpVelocity;
            }

            if (!isGrounded)
            {
                velocity.y = mover.GetVelocity().y;
                velocity.y -= gravity * Time.fixedDeltaTime;
            }








            mover.SetCheckGround(jumpInput);


            mover.SetExtendSensorRange(isGrounded);
            mover.SetVelocity(velocity);
        }

        private void LateUpdate()
        {
            Vector3 groundNormal = mover.GetGroundNormal();

            Quaternion targetRotationY = Quaternion.AngleAxis(rotationSpeed * rotationInput.x,groundNormal);
            Quaternion targetRotationGround = Quaternion.FromToRotation(transform.up, groundNormal);

            Quaternion finalRotation = targetRotationY * targetRotationGround  * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationLerpSpeed * Time.deltaTime);

        }

    }
}