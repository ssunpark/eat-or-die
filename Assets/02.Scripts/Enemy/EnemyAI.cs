using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachineController))]
public class EnemyAI : NetworkBehaviour, IStateMachineOwner, IMoveable, IDetector, IAttackable
{
	public int EnemyID; // 몬스터 ID

	[Networked] public EAnimationState AnimationState { get; set; }
	private bool _hit;
	
	public NetworkObject NetworkObject => Object;

	private Animator _animator;

	private ChangeDetector _changeDetector;
	
	public EnemyStatManager EnemyStatManager;

	private RangeDetector _raycastComponent;

	[SerializeField] private float _currentHunger;
	
	public EnemyContext Context { get; private set; }

	[SerializeField] private SpawnBehaviour _spawnBehaviour;
	[SerializeField] private IdleBehaviour _idleBehaviour;
	[SerializeField] private MoveBehaviour _moveBehaviour;
	[SerializeField] private AttackBehaviour _attackBehaviour;
	[SerializeField] private HitBehaviour _hitBehaviour;
	[SerializeField] private DieBehaviour _dieBehaviour;


    private EnemyBehaviourMachine _behaviourMachine;

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private int _areaMask = NavMesh.AllAreas;
    public override async void Spawned()
    {
		_raycastComponent = GetComponent<RangeDetector>();
		_raycastComponent.Radius = Context.StatManager.GetStat(EStatType.EnemyDetectionRange);
		_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		
		if(Context.Agent != null)
		{
			Context.Agent.enabled = false;
            Context.Agent.updatePosition = false;
            Context.Agent.updateRotation = false;
            Context.Agent.updateUpAxis = false;
        }
        if (Object.HasStateAuthority)
        {
            // 2) 유효 위치 스냅
            var pos = transform.position;
            if (NavMesh.SamplePosition(pos, out var hit, 2.0f, _areaMask))
            {
                transform.position = hit.position;
            }

			_agent = Context.Agent;
            // 3) 한 틱/한 프레임 대기 후 활성화 & 워프
            await UniTask.NextFrame();
            if (_agent != null)
            {
                _agent.Warp(transform.position);
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.nextPosition = transform.position;
            }
        }
        else
        {
            // 프록시: NavMeshAgent는 계속 꺼두고, 네트워크 동기화만으로 렌더
            if (_agent != null)
            {
                _agent.enabled = false;
            }
        }
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		EnemyStatManager = new EnemyStatManager(EnemyID);

		_currentHunger = EnemyStatManager.GetStat(EStatType.EnemyHunger);
		_animator = GetComponent<Animator>();
		
		Context = new EnemyContext()
		{
			Owner = this,
			Target = null,
			Animator = _animator,
			StatManager = EnemyStatManager,
			AnimationRelay = GetComponent<EnemyAnimationRelay>(),
			Agent = GetComponent<NavMeshAgent>(),
			Mover = this,
			Detector = this,
			RaycastComponent = GetComponent<RangeDetector>(),
		};
		
		AEnemyStateBehaviour[] stateList = new AEnemyStateBehaviour[]
		{
			_spawnBehaviour,
			_idleBehaviour,
			_moveBehaviour,
			_attackBehaviour,
			_hitBehaviour,
			_dieBehaviour,
		};
		
		_behaviourMachine = new EnemyBehaviourMachine("Behaviour Machine", Context, stateList);
		
		Context.Agent.speed = Context.StatManager.GetStat(EStatType.EnemyMoveSpeed);
		Context.AnimationRelay.SetMachine(_behaviourMachine);
		
		stateMachines.Add(_behaviourMachine);
	}

	public override void FixedUpdateNetwork()
	{
		if (_behaviourMachine.ActiveState is DieBehaviour) return;
		
		if (_hit)
		{
			if (_currentHunger <= 0)
			{
				_behaviourMachine.ForceActivateState<DieBehaviour>();
				return;
			}
			_behaviourMachine.ForceActivateState<HitBehaviour>();
			_hit = false;
		}
	}
	
	public override void Render()
	{
		foreach (string change in _changeDetector.DetectChanges(this))
		{
			switch (change)
			{
				case nameof(AnimationState):
					PlayAnimation();
					break;
			}
		}
	}

	private void PlayAnimation()
	{
		if (_animator == null) return;
		
		string stateName = AnimationState.ToString();
		_animator.CrossFade(stateName, 0.1f);
	}

	public void Move()
	{
        if (!Object.HasStateAuthority || _agent == null || !_agent.enabled) return;
        if (Context.Agent.pathPending || !Context.Agent.hasPath) return;
		
		Vector3 direction = Context.Agent.nextPosition - transform.position;
		transform.forward = direction.normalized;
		
		if (direction.sqrMagnitude < 0.01f) return;
		
		direction.Normalize();

		transform.position += direction * Context.Agent.speed * Runner.DeltaTime;
	}

	public bool Detect()
	{
		Collider closestTarget = _raycastComponent.NearestMember;
		
		float distance = Vector3.Distance(transform.position, closestTarget.transform.position);
		
		Player closestPlayer = closestTarget.GetComponent<Player>();
		
		if (distance <= _raycastComponent.Radius && !closestPlayer.IsDead)
		{
			Context.Target = closestPlayer;
			_behaviourMachine.TryActivateState<MoveBehaviour>();
			return true;
		}

		return false;
	}

	public void OnHitLocal(AttackInfo attack)
	{
		if (HasStateAuthority)
		{
			float meleeAmount = attack.MeleeDamage * attack.TotalDamageMultiplier;
			float magicAmount = attack.MagicDamage * attack.TotalDamageMultiplier;
			float meleeDefense = EnemyStatManager.GetStat(EStatType.EnemyMeleeDefense);
			float magicDefense = EnemyStatManager.GetStat(EStatType.EnemyMagicDefense);
			
			float totalDamage = meleeAmount * (100 / (100 + meleeDefense)) + magicAmount * (100 / (100 + magicDefense));
			ParticleManager.Instance.DamageSpawn(totalDamage, transform.position + Vector3.up * 0.5f, EDamageFloaterType.Damage, true);
            _currentHunger -= totalDamage;
		
			_hit = true;
			
			Context.Target = attack.Attacker.GetComponent<Player>();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void RPC_HitByAttack(AttackInfo attack)
	{
	}

	public void OnHitStateAuthority(AttackInfo attack)
	{
	}
}
