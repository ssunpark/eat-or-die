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
    public PlayerInputActions InputActions => _inputActions;

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
    }

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
