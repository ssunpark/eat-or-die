using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    private PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        _inputActions.Player.Move.performed += HandleMovePerformed;
        _inputActions.Player.Move.canceled += HandleMoveCanceled;
        _inputActions.Player.Attack.performed += ctx => _attackPressed = true; 
        _inputActions.Player.Jump.performed += ctx => _jumpPressed = true;
    }
    private bool _attackPressed;
    private bool _jumpPressed;
    public bool ConsumeAttackInput()
    {
        bool result = _attackPressed;
        _attackPressed = false;
        return result;
    }
    public bool ConsumeJumpInput()
    {
        bool result = _jumpPressed;
        _jumpPressed = false;
        return result;
    }

    public bool IsRunning => _inputActions.Player.Sprint.IsPressed();
    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= HandleMovePerformed;
        _inputActions.Player.Move.canceled -= HandleMoveCanceled;
        _inputActions.Player.Disable();
    }
}
