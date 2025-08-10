using UnityEngine;
using UnityEngine.AI;

public class DragonMovement
{
    private readonly DragonController _controller;
    private Vector2 _smoothedVelocity;
    private bool _isLocked = false;

    public NavMeshAgent NavMeshAgent { get; private set; }

    public DragonMovement(DragonController controller)
    {
        _controller = controller;
        NavMeshAgent = _controller.GetComponent<NavMeshAgent>();
    }

    public void OnSpawned()
    {
        var baseParams = _controller.ParamLoader?.Base;

        NavMeshAgent.updatePosition = false;
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;

        if (baseParams != null)
        {
            NavMeshAgent.speed = baseParams.MoveSpeed;
            NavMeshAgent.angularSpeed = baseParams.RotationSpeed;
        }
    }

    public void Move(float dt)
    {
        if (_isLocked)
            return;

        if (NavMeshAgent.pathPending)
            return;

        _controller.transform.position = NavMeshAgent.nextPosition;

        Vector3 direction = _controller.Target != null
            ? _controller.Target.transform.position - _controller.transform.position
            : NavMeshAgent.steeringTarget - _controller.transform.position;

        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(direction.normalized);
            _controller.transform.rotation = Quaternion.RotateTowards(
                _controller.transform.rotation,
                rot,
                NavMeshAgent.angularSpeed * dt);
        }

        Vector3 localVelocity = _controller.transform.InverseTransformDirection(NavMeshAgent.desiredVelocity);
        Vector2 targetVelocity = new(localVelocity.x, localVelocity.z);

        var baseParams = _controller.ParamLoader?.Base;
        float smooth = baseParams?.AnimSmoothSpeed ?? 1f;

        if (_controller.HasStateAuthority)
        {
            _smoothedVelocity = Vector2.Lerp(_smoothedVelocity, targetVelocity, smooth * dt);
            _controller.AnimVelocity = _smoothedVelocity; // 네트워크로 복제될 값
        }
    }

    public void MaintainDistanceAndLookAtTarget(float dt, float desiredDistance)
    {
        if (_isLocked || _controller.Target == null)
            return;

        Vector3 dir = _controller.transform.position - _controller.Target.transform.position;
        dir.y = 0f;

        float dist = dir.magnitude;
        if (dist < 0.01f)
            return;

        Quaternion rot = Quaternion.LookRotation(-dir.normalized);
        _controller.transform.rotation = Quaternion.RotateTowards(
            _controller.transform.rotation, rot,
            NavMeshAgent.angularSpeed / 2f * dt);

        if (dist < desiredDistance)
        {
            Vector3 back = -_controller.transform.forward;
            Vector3 movePos = _controller.transform.position + back * (desiredDistance - dist);
            movePos.y = _controller.transform.position.y;

            _controller.transform.position = Vector3.MoveTowards(
                _controller.transform.position, movePos,
                NavMeshAgent.speed * dt);
        }
    }

    public void SetDestination(Vector3 position)
    {
        if (_isLocked || !NavMeshAgent.enabled)
            return;
        NavMeshAgent.isStopped = false;
        NavMeshAgent.SetDestination(position);
    }

    public void ResetNavMeshAgent()
    {
        NavMeshAgent.ResetPath();
        NavMeshAgent.isStopped = true;
        NavMeshAgent.velocity = Vector3.zero;
        NavMeshAgent.nextPosition = _controller.transform.position;
    }

    public void SetNavMeshAgentMoveData(float moveSpeed, float angularSpeed)
    {
        NavMeshAgent.speed = moveSpeed;
        NavMeshAgent.angularSpeed = angularSpeed;
    }
    
    public bool Arrived()
    {
        return !NavMeshAgent.pathPending &&
               NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance;
    }

    public void Lock()
    {
        _isLocked = true;

        if (!NavMeshAgent.enabled)
            return;
        ResetNavMeshAgent();
    }

    public void Unlock()
    {
        _isLocked = false;
    }

    public bool IsLocked => _isLocked;
}