using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : CharacterBase
{
    private NetworkCharacterController _characterController;
    private bool _isSpawned = false;
    private SatietyEffectHandler _satietyEffectHandler;
    private PlayerStateMachine _fsm;

    [HideInInspector] public PlayerAnimator PlayerAnimatorController;
    public SatietyEffectHandler SatietyEffectHandler => _satietyEffectHandler;
    [Networked]public bool IsAttacking { get; set; }

    private float _lastAttackTime;
    public float LastAttackTime
    {
        get => _lastAttackTime;
        set
        {
            _lastAttackTime = value;
        }
    }

    [SerializeField] private string _playerHUDTagName = "PlayerHUD";

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetIsAttacking(bool attacking)
    {
        IsAttacking = attacking;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Room.Instance.SetLocalPlayer(gameObject);
        }
        _fsm = GetComponent<PlayerStateMachine>();
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        _satietyEffectHandler = new SatietyEffectHandler(Resource, Stat);
        _isSpawned = true;
        TryInitialize();
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

            if (TryGetComponent(out PlayerMove playerMove))
            {
                playerMove.Initialize(Stat, _fsm, _characterController, this, Resource);
            }
            else
            {
                Debug.LogError("PlayerMove component not found!!");
            }
        }
    }


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

}
