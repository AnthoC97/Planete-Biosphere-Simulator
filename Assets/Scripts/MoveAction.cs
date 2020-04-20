using UnityEngine;

public class MoveAction : Action
{
    private Vector3 destination;
    private GameObject actor;

    public MoveAction(Vector3 destination, GameObject actor)
    {
        this.destination = destination;
        this.actor = actor;
    }

    public override (bool done, bool result) Execute()
    {
        float speed = .5f;

        actor.transform.position =
            Vector3.MoveTowards(actor.transform.position, destination, speed);

        float distance =
            Vector3.Distance(actor.transform.position, destination);

        if (distance < .000001f) {
            return (true, true);
        }

        return (false, false);
    }

    public override bool CanBeExecuted()
    {
        return true;
    }

    public override void Cancel()
    {

    }
}
