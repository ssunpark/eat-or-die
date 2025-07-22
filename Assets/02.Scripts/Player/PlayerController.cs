using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : CharacterBase
{
    [HideInInspector] public PlayerAnimator PlayerAnimatorController;

    private NetworkCharacterController _characterController;
    private bool _isSpawned = false;
    private SatietyEffectHandler _satietyEffectHandler;
    public SatietyEffectHandler SatietyEffectHandler => _satietyEffectHandler;

    private PlayerStateMachine _fsm;

    private bool _isAttackingLocally = false;
    public bool IsLocalAttackLocked => _isAttackingLocally;

    private float _lastAttackTime;
    public float AttackCooldown = 1f;

    [SerializeField] private string _playerHUDTagName = "PlayerHUD";

    public override void Spawned()
    {
        _fsm = GetComponent<PlayerStateMachine>();
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        _satietyEffectHandler = new SatietyEffectHandler(Resource, Stat);
        _isSpawned = true;
        TryInitialize();
    }

    public void SetLocalAttackLock(bool p)
    {
        _isAttackingLocally = p;
    }
    private void InitializePlayerHUD()
    {
        // 나중에 UIManager를 통해 HUD를 관리할 예정
        GameObject hudObject = GameObject.FindGameObjectWithTag(_playerHUDTagName);
        if (hudObject != null)
        {
            UI_HUDPlayerHP hudHP = hudObject.GetComponent<UI_HUDPlayerHP>();
            if (hudHP != null)
            {
                hudHP.Initialize(Resource, Stat); // ResourceManager와 StatManager 전달
            }
            else
            {
                Debug.LogError($"HUD 오브젝트 '{_playerHUDTagName}'에 UI_HUDPlayerHP 스크립트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"씬에서 태그 '{_playerHUDTagName}'를 가진 HUD 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void TryInitialize()
    {
        if (_isSpawned)
        {
            _characterController.maxSpeed = Stat.GetStat(EStatType.MoveSpeed);
            _characterController.jumpImpulse = Stat.GetStat(EStatType.JumpPower);
            _characterController.acceleration = Stat.GetStat(EStatType.Acceleration);


            if (Object.HasInputAuthority)
            {
                InitializePlayerHUD();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_fsm == null)
        {
            return;
        }

        if (GetInput(out NetworkInputData inputData))
        {
            HandleMove(inputData);

            HandleJump(inputData);
        }
    }

    private void HandleMove(NetworkInputData inputData)
    {
        Vector3 moveDirection = inputData.direction;

        if (moveDirection.sqrMagnitude > 0.01f)
        {

            float baseSpeed = Stat.GetStat(EStatType.MoveSpeed);
            float sprintMultiplier = inputData.isRunning
                ? Stat.GetStat(EStatType.SprintingMultiplier)
                : 1f;


            float moveSpeed = _fsm.CanMove && !_isAttackingLocally ? (baseSpeed * sprintMultiplier) : 0f;
            if (moveSpeed > 0f)
            {
                Resource.ConsumeSatiety(Time.deltaTime * Stat.GetStat(EStatType.ConsumptionRate));
            }
            if (_characterController.maxSpeed != moveSpeed)
            {
                _characterController.maxSpeed = moveSpeed;
            }
            _characterController.Move(moveDirection);


        }
        else
        {
            _characterController.Move(Vector3.zero);
        }
    }

    private void HandleJump(NetworkInputData inputData)
    {
        if (inputData.isJumping && _characterController.Grounded)
        {
            float jumpPower = Stat.GetStat(EStatType.JumpPower);
            _characterController.jumpImpulse = jumpPower;
            _characterController.Jump();
            if (Object.HasInputAuthority)
            {
                Rpc_PlayAnimTrigger(EAnimTrigger.Jump);
            }
        }
    }

    public bool IsGrounded => _characterController.Grounded;

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_PlayAnimTrigger(EAnimTrigger trigger)
    {
        PlayerAnimatorController.PlayTrigger(trigger);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_DealDamage(NetworkObject target, int amount)
    {
        if (target == null) return;

        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(amount, Object.InputAuthority);
        }
    }

}
