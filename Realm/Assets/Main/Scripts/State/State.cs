using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnEnter();

    public void OnExit();

    public void OnUpdate();
}


public abstract class State<T> : IState where T : Component
{

    protected T target;

    public State(T target)
    {
        this.target = target;
    }

    public virtual void OnEnter()
    {
        Debug.Log($"{this.GetType().Name} 호출");
    }

    public virtual void OnExit()
    {
        Debug.Log($"{this.GetType().Name} 종료");
    }

    public virtual void OnUpdate()
    {
        Debug.Log($"{this.GetType().Name} 업데이트");
    }

}
