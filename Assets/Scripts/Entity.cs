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

    public (bool done, bool result, Action action) ExecuteCurrentAction()
    {
        Action currentAction = actionsQueue[0];

        if (!currentAction.CanBeExecuted()) {
            actionsQueue.RemoveAt(0);
            return (true, false, currentAction);
        }

        var status = currentAction.Execute();

        if (status.done) {
            actionsQueue.RemoveAt(0);
        }

        return (status.done, status.result, currentAction);
    }
}
