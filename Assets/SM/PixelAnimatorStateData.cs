using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PixelAnimatorStateData : ScriptableObject
{
    public string stateName;
    public abstract PixelAnimatorState ConstructState(PixelAnimator animator);
}

public abstract class PixelAnimatorState
{
    public string name;
    public PixelAnimatorStateData data;
    public PixelAnimatorState(string name, PixelAnimatorStateData data)
    {
        this.name = name;
        this.data = data;
    }

    public virtual void OnEnter(PixelAnimator animator) { }
    public virtual void OnUpdate(PixelAnimator animator) { }
    public virtual void OnExit(PixelAnimator animator) { }
}

// Class Template:
/*
[CreateAssetMenu(menuName = "SM/")]
public class StateData : PixelAnimatorStateData
{
    //[Header("Animations")]
    

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new State(stateName, this, animator);
    }
}

public class State : PixelAnimatorState
{
    new StateData data;
    PixelAnimator animator;

    public State(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as StateData;

    }
}
*/