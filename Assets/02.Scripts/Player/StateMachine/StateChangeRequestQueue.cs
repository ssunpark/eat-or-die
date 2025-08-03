using System.Collections.Generic;
using Fusion.Addons.FSM;

public class StateChangeRequestQueue<TState> where TState : class, IState
{
    private Queue<TState> _queue = new Queue<TState>();

    public void Request(TState state)
    {
        if (!_queue.Contains(state))
            _queue.Enqueue(state);
    }

    public void ExecuteAll(StateMachine<TState> fsm)
    {
        while (_queue.Count > 0)
        {
            var next = _queue.Dequeue();
            fsm.TryActivateState(next,true);
        }
    }

    public void ForceOverride(TState state)
    {
        _queue.Clear();
        _queue.Enqueue(state);
    }
}
