using UnityEngine;

public class ActionGetAsleep : Action
{
    private const int framesToGetAsleep = 10;

    private int framesElapsed = 0;
    private IA artificialIntelligence;

    public ActionGetAsleep(IA artificialIntelligence)
    {
        this.artificialIntelligence = artificialIntelligence;
    }

    public override (bool done, bool result) Execute()
    {
        framesElapsed += 1;

        if (framesElapsed > framesToGetAsleep) {
            artificialIntelligence.currentState = IA.state.sleeping;
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
