using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerControls playerControls;
    Vector2 currentMovementInput;
    [HideInInspector]
    public Vector3 CurrentMovement;
    public float AccelerationInput { get => accelerationInput; }
    float accelerationInput;
    public float BrakeInput { get => brakeInput; }
    float brakeInput;

    public bool IsJumpPressed { get => isJumpPressed; }
    bool isJumpPressed = false;
    public bool RequireNewJumpPress { set => requireNewJumpPress = value; get => requireNewJumpPress; }
    bool requireNewJumpPress = false;

    public bool IsShooting { get => isShooting; }
    bool isShooting = false;

    private void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Player.Move.started += OnMovementInput;
        playerControls.Player.Move.canceled += OnMovementInput;
        playerControls.Player.Move.performed += OnMovementInput;

        playerControls.Player.Accelerate.started += ctx => accelerationInput = ctx.ReadValue<float>();
        playerControls.Player.Accelerate.canceled += ctx => accelerationInput = ctx.ReadValue<float>();
        playerControls.Player.Accelerate.performed += ctx => accelerationInput = ctx.ReadValue<float>();

        playerControls.Player.Brake.started += ctx => brakeInput = ctx.ReadValue<float>();
        playerControls.Player.Brake.canceled += ctx => brakeInput = ctx.ReadValue<float>();
        playerControls.Player.Brake.performed += ctx => brakeInput = ctx.ReadValue<float>();

        playerControls.Player.Jump.started +=
            ctx => { isJumpPressed = ctx.ReadValueAsButton(); requireNewJumpPress = false; };
        playerControls.Player.Jump.canceled +=
            ctx => { isJumpPressed = ctx.ReadValueAsButton(); requireNewJumpPress = false; };

        playerControls.Player.Shoot.started += ctx => isShooting = ctx.ReadValueAsButton();
        playerControls.Player.Shoot.canceled += ctx => isShooting = ctx.ReadValueAsButton();
    }
    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }



    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        CurrentMovement.x = currentMovementInput.x;
        CurrentMovement.z = currentMovementInput.y;
        CurrentMovement.y = 0f;

    }

}