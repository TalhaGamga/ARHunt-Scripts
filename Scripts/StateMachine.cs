using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class StateMachine
{
    public string currentStateType;

    private List<StateTransition> stateTransitions;

    private IState currentState;

    private Dictionary<ITriggerableWrapper, Delegate> triggerBindings = new Dictionary<ITriggerableWrapper, Delegate>();

    public StateMachine()
    {
        stateTransitions = new List<StateTransition>();
    }

    public void SetState(IState newState)
    {
        if (newState.Equals(currentState))
        {
            return;
        }

        currentState?.Exit();
        newState?.Enter();
        currentState = newState;
        currentStateType = currentState.type;
    }

    private void SetState(StateTransitionData transitionData)
    {
        if (transitionData.to.Equals(currentState))
        {
            return;
        }

        currentState?.Exit();
        transitionData.onTransition?.Invoke();
        transitionData.to?.Enter();
        currentState = transitionData.to;
        currentStateType = currentState.type;
    }

    public void Update()
    {
        StateTransitionData transitionData = checkTransitions();

        if (transitionData != null)
        {
            SetState(transitionData);
        }

        currentState?.Update();
    }

    public void AddNormalTransition(StateTransition stateTransition)
    {
        stateTransitions.Add(stateTransition);
    }

    public void AddAnyTransition(StateTransition anyTransition)
    {
        stateTransitions.Add(anyTransition);
    }

    public void AddAnyTransitionTrigger<T>(TriggerableWrapper<T> trigger, StateTransition transition)
    {
        Action<T> handler = (T) =>
        {
            if (transition.Condition() && !currentState.Equals(transition.To))
            {
                SetState(transition.To);
                transition.OnTransition?.Invoke();
            }
        };

        trigger.OnTrigger += handler;

        triggerBindings[trigger] = handler;
    }

    public void RemoveTrigger<T>(TriggerableWrapper<T> trigger)
    {
        if (triggerBindings.TryGetValue(trigger, out var handler))
        {
            trigger.OnTrigger -= (Action<T>)handler;
            triggerBindings.Remove(trigger);
        }
    }

    public void AddNormalTransitionTrigger<T>(TriggerableWrapper<T> trigger, StateTransition transition)
    {
        Action<T> handler = (T) =>
        {
            if (transition.Condition() && currentState.Equals(transition.From) && !currentState.Equals(transition.To))
            {
                SetState(transition.To);
                transition.OnTransition?.Invoke();
            }
        };

        trigger.OnTrigger += handler;

        if (!triggerBindings.ContainsKey(trigger))
        {
            triggerBindings[trigger] = handler;
        }
    }

    private StateTransitionData checkTransitions()
    {
        foreach (var transition in stateTransitions.OrderByDescending(t => t.Priority))
        {
            if (transition.From != null)
            {
                if (transition.From.Equals(currentState))
                {
                    if (transition.Condition.Invoke())
                    {
                        return new StateTransitionData(transition.To, transition.OnTransition);
                    }

                    return null;
                }
            }

            else
            {
                if (transition.Condition.Invoke())
                {
                    return new StateTransitionData(transition.To, transition.OnTransition);
                }
            }
        }

        return null;
    }
}

public class StateTransition
{
    private IState _from;
    private IState _to;
    private Func<bool> _condition;
    private Action onTransition;
    private int _priority { get; set; }

    // Transition between a specific "From" state to a "To" state when the condition is met.
    // This transition uses a priority to determine which transition takes precedence.
    public StateTransition(IState from, IState to, Func<bool> condition, int priority)
    {
        _from = from;
        _to = to;
        _condition = condition;
        _priority = priority;
    }

    // Transition between a specific "From" state to a "To" state when triggered and the condition is met.
    // Use this for standard state-to-state transitions without priority.
    public StateTransition(IState from, IState to, Func<bool> condition)
    {
        _from = from;
        _to = to;
        _condition = condition;
    }

    // Transition from any state to the "To" state when the condition is met.
    // This transition uses a priority to control its precedence over other transitions.
    public StateTransition(IState to, Func<bool> condition, int priority)
    {
        _to = to;
        _condition = condition;
        _priority = priority;
    }

    // Transition from any state to the "To" state when triggered and the condition is met.
    // Use this when the transition is not restricted to a specific "From" state and priority is not needed.
    public StateTransition(IState to, Func<bool> condition)
    {
        _to = to;
        _condition = condition;
    }

    // Transition between a specific "From" state to a "To" state.
    // This transition does not require a condition or priority and is triggered directly.
    public StateTransition(IState from, IState to)
    {
        _from = from;
        _to = to;
    }

    // Transition from any state to the "To" state.
    // This transition does not require a condition or priority and is triggered directly.
    public StateTransition(IState to)
    {
        _to = to;
    }

    public IState From
    {
        get { return _from; }
    }

    public IState To
    {
        get { return _to; }
    }

    public Func<bool> Condition
    {
        get { return _condition; }
    }

    public Action OnTransition
    {
        get
        {
            return onTransition;
        }
    }

    public int Priority
    {
        get
        {
            return _priority;
        }
    }

    public void SetOnTransition(Action transitionAction)
    {
        onTransition = transitionAction;
    }
}

public class StateTransitionData
{
    public IState to;
    public Action onTransition;

    public StateTransitionData(IState to, Action onTransition)
    {
        this.to = to;
        this.onTransition += onTransition;
    }
}

public interface IState
{
    public string type { get; set; }
    public Action<object?> OnStateEnter { get; set; }
    public Action<object?> OnStateExit { get; set; }
    public Action<object?> OnStateUpdate { get; set; }
    void Enter();
    void Update();
    void Tick();
    void Exit();
}