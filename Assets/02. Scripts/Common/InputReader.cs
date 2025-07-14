using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    private PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

        _inputActions.Player.Move.performed += HandleMovePerformed;
        _inputActions.Player.Move.canceled += HandleMoveCanceled;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

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
