using UnityEngine;
using MoonSharp.Interpreter;

public abstract class Action
{
    public System.Action OnFailureCallback;

    public abstract (bool done, bool result) Execute();
    public abstract bool CanBeExecuted();
    public abstract void Cancel();

    public void OnFailure(System.Action action)
    {
        OnFailureCallback = action;
    }

    public void OnFailure(Closure closure)
    {
        OnFailureCallback = () => closure.Call();
    }
}
