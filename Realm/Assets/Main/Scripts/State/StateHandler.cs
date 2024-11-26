using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.CullingGroup;

public abstract class StateHandler<T> :MonoBehaviour
{
    public T target;
    
    public IState CurrentState {  get; protected set; }

    public StateHandler(T target)
    {
        this.target = target;
    }

    protected event Action<IState> OnStateChanged;

    public virtual void Initialize()
    {

        OnStateChanged?.Invoke(CurrentState);
    }

    public void TransitionTo(IState nextstate)
    {
        CurrentState.OnExit();
        CurrentState = nextstate;
        CurrentState.OnEnter();

        OnStateChanged?.Invoke(nextstate);
    }

    public virtual void HandleUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }

}