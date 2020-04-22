using UnityEngine;

public class ActionDrink : Action
{
    private const int framesToDrink = 10;

    private int framesElapsed = 0;
    private IA artificialIntelligence;
    private GameObject water;

    public ActionDrink(IA artificialIntelligence, GameObject water)
    {
        this.artificialIntelligence = artificialIntelligence;
        this.water = water;
    }

    public override (bool done, bool result) Execute()
    {
        // TODO Check if water is null
        framesElapsed += 1;

        if (framesElapsed > framesToDrink) {
            artificialIntelligence.thirst = 0;
            artificialIntelligence.currentState = IA.state.idle;
            GameObject.Destroy(water);
            return (true, true);
        }

        return (false, false);
    }

    public override bool CanBeExecuted()
    {
        return Vector3.Distance(water.transform.position,
                artificialIntelligence.gameObject.transform.position) < 1;
    }

    public override void Cancel()
    {

    }
}
