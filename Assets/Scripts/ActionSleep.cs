using UnityEngine;

public class ActionSleep : Action
{
    private IA artificialIntelligence;

    public ActionSleep(IA artificialIntelligence)
    {
        this.artificialIntelligence = artificialIntelligence;
    }

    public override (bool done, bool result) Execute()
    {
        if (artificialIntelligence.stamina < 100) {
            artificialIntelligence.stamina += 1;
        }

        if (artificialIntelligence.stamina >= 100) {
            artificialIntelligence.currentState = IA.state.idle;
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
