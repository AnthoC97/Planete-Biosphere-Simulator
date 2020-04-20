using UnityEngine;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    private List<Action> actionsQueue = new List<Action>();

    public bool HasActionsQueued()
    {
        return actionsQueue.Count > 0;
    }

    public void AddAction(Action action)
    {
        actionsQueue.Add(action);
    }

    public bool CancelAction(Action action)
    {
        if (!actionsQueue.Contains(action)) {
            return false;
        }

        actionsQueue.Remove(action);

        return true;
    }

    public (bool, bool) ExecuteCurrentAction()
    {
        /* TODO Discuss whether actions that cannot be executed should be
           removed from the queue */
        if (!actionsQueue[0].CanBeExecuted()) {
            actionsQueue.RemoveAt(0);
            return (true, false);
        }

        var status = actionsQueue[0].Execute();

        if (status.done) {
            actionsQueue.RemoveAt(0);
        }

        return status;
    }
}
