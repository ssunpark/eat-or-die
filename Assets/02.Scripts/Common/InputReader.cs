using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : BehaviourSingleton<InputReader>
{
    public Vector2 MoveInput { get; private set; }
    public bool IsAttackDown { get; private set; }
    public bool IsInteractDown { get; private set; }
    public bool IsEscapeDown { get; private set; }
    public bool IsUseItemDown { get; private set; }
    public bool IsSprintDown { get; private set; }

    public Vector3 MousePosition { get; private set; }
    private PlayerInputActions _inputActions;
    public PlayerInputActions InputActions => _inputActions;
    public LayerMask GroundLayerMask;

    protected bool _externalInputBlocked;
    private bool _paused;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _externalInputBlocked = false;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        _inputActions.Player.Move.performed += HandleMovePerformed;
        _inputActions.Player.Move.canceled += HandleMoveCanceled;
        _inputActions.Player.Attack.performed += OnAttackPerformed;
        _inputActions.Player.Interact.performed += OnInteractPerformed;
        _inputActions.Player.UseItem.performed += OnUseItemPerformed;
        _inputActions.Player.Sprint.performed += OnSprintPerformed;

    }
    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        IsAttackDown = context.action.IsPressed();
        MousePosition = GetMousePosition();
    }

    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, GroundLayerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        IsInteractDown = context.action.IsPressed();
    }
    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        IsEscapeDown = context.action.IsPressed();
    }
    private void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        IsUseItemDown = context.action.IsPressed();
        MousePosition = GetMousePosition();
    }
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        IsSprintDown = context.action.IsPressed();
    }


    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= HandleMovePerformed;
        _inputActions.Player.Move.canceled -= HandleMoveCanceled;
        _inputActions.Player.Attack.performed -= OnAttackPerformed;
        _inputActions.Player.Interact.performed -= OnInteractPerformed;
        _inputActions.Player.UseItem.performed -= OnUseItemPerformed;

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
