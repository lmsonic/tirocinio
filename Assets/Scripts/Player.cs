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
    bool isAccelerating = false;
    bool isBraking = false;

    float verticalVelocity = 0f;
    Vector3 groundVelocity = Vector3.zero;

    Transform cameraTransform;

    public float gravity = -10f;
    public float jumpHeight = -10f;

    public float acceleration = 10f;
    public float deceleration = 5f;
    public float brakeSpeed = 20f;
    public float rotationSpeed = 0.8f;
    public float steerRotationSpeed = 0.8f;
    public float maxSteerAngle = 20f;
    public float rotationFollowBodySpeed = 0.4f;
    public float maxVelocity = 20f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform handleTransform;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }


    private void Update()
    {

        VerticalMovement();
        HorizontalMovement();

        Rotation();




    }

    void Rotation()
    {

        float targetAngle = cameraTransform.eulerAngles.y;

        float steerAngle = GetSteerAngle();

        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, steerAngle);
        handleTransform.rotation = Quaternion.Lerp(handleTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationFollowBodySpeed * Time.deltaTime);


    }

    float GetSteerAngle(){

        Vector3 movement = GetInputMovement();
        float targetSteerAngle = -movement.x * maxSteerAngle;
        float steerAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetSteerAngle, steerRotationSpeed);
        
        return steerAngle;
    }

    void HorizontalMovement()
    {
        Vector3 movement = GetInputMovement();

        movement = movement.x * cameraTransform.right.normalized + movement.z * cameraTransform.forward.normalized;

        if (isAccelerating)
        {
            groundVelocity = Vector3.Lerp(groundVelocity, handleTransform.forward * maxVelocity, acceleration * Time.deltaTime);
        }
        else if (isBraking)
        {
            groundVelocity = Vector3.Lerp(groundVelocity, Vector3.zero, brakeSpeed * Time.deltaTime);
        }
        else
        {
            groundVelocity = Vector3.Lerp(groundVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        controller.Move(groundVelocity * Time.deltaTime);


    }

    Vector3 GetInputMovement()
    {

        return new Vector3(inputMovement.x, 0f, inputMovement.y);
    }

    void VerticalMovement()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = 0f;

        if (isJumping && isGrounded)
        {
            verticalVelocity += Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = false;
        }

        if (!isGrounded)
            verticalVelocity += gravity * Time.deltaTime;

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

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

    public void Accelerate(InputAction.CallbackContext ctx)
    {
        isAccelerating = ctx.performed;
    }

    public void Brake(InputAction.CallbackContext ctx)
    {
        isBraking = ctx.performed;
    }
}
