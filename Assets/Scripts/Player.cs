using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController controller;
    Vector2 inputMovement = Vector2.zero;
    Vector2 inputLook = Vector2.zero;

    bool isJumping = false;

    Vector3 velocity = Vector3.zero;

    Transform cameraTransform;

    public float gravity = -10f;
    public float jumpHeight = -10f;
    public float rotationSpeed = 0.8f;
    public float moveSpeed = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }


    private void Update()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded && velocity.y < 0)
            velocity.y = 0f;

        Vector3 movement = new Vector3(inputMovement.x, 0f, inputMovement.y);

        movement = movement.x * cameraTransform.right.normalized + movement.z * cameraTransform.forward.normalized;
        controller.Move(movement * moveSpeed * Time.deltaTime);

        if (isJumping && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = false;
        }

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


    }



    public void Move(InputAction.CallbackContext ctx)
    {

        inputMovement = ctx.ReadValue<Vector2>();

    }
    public void Look(InputAction.CallbackContext ctx)
    {
        inputLook = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        isJumping = ctx.performed;
    }
}
