using UnityEngine;

public abstract class Action
{
    public abstract (bool done, bool result) Execute();
    public abstract bool CanBeExecuted();
    public abstract void Cancel();
}
