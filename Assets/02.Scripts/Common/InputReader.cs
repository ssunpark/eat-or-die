using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { 
        get
        {
            if (playerControllerInputBlocked || _externalInputBlocked)
            {
                return Vector2.zero;
            }
            return _moveInput;
        }
    }

    private Vector2 _moveInput;
    private PlayerInputActions _inputActions;

    [HideInInspector]
    public static bool playerControllerInputBlocked;
    protected bool _externalInputBlocked;
    private bool _paused;

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
        _inputActions.Player.Interact.performed += ctx => _interactPressed = true;
        _inputActions.Player.UseItem.performed += ctx => _useItemPressed = true;
    }
    private bool _attackPressed;
    private bool _jumpPressed;

    private bool _interactPressed; 
    private bool _useItemPressed;

    public bool UseItemInput
    {
        get { return _useItemPressed && !playerControllerInputBlocked && !_externalInputBlocked; }
    }
    public bool JumpInput
    {
        get { return _jumpPressed && !playerControllerInputBlocked && !_externalInputBlocked; }
    }

    public bool AttackInput
    {
        get { return _attackPressed && !playerControllerInputBlocked && !_externalInputBlocked; }
    }

    public bool InteractInput
    {
        get { return _interactPressed && !playerControllerInputBlocked && !_externalInputBlocked; }
    }
    public bool ConsumeUseItemInput()
    {
        if (playerControllerInputBlocked || _externalInputBlocked)
        {
            return false;
        }
        bool result = UseItemInput;
        _useItemPressed = false;
        return result;
    }
    public bool ConsumeInteractionInput()
    {
        if (playerControllerInputBlocked || _externalInputBlocked)
        {
            return false;
        }
        bool result = InteractInput;
        _interactPressed = false;
        return result;
    }

    public bool ConsumeAttackInput()
    {
        if (playerControllerInputBlocked || _externalInputBlocked)
        {
            return false;
        }
        bool result = AttackInput;
        _attackPressed = false;
        return result;
    }
    public bool ConsumeJumpInput()
    {
        if (playerControllerInputBlocked || _externalInputBlocked)
        {
            return false;
        }
        bool result = JumpInput;
        _jumpPressed = false;
        return result;
    }

    public bool IsRunning => _inputActions.Player.Sprint.IsPressed();
    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= HandleMovePerformed;
        _inputActions.Player.Move.canceled -= HandleMoveCanceled;
        _inputActions.Player.Disable();
    }

    public bool Pause
    {
        get { return _paused; }
    }


    public bool HaveControl()
    {
        return !_externalInputBlocked;
    }

    public void ReleaseControl()
    {
        _externalInputBlocked = true;
    }

    public void GainControl()
    {
        _externalInputBlocked = false;
    }
}
