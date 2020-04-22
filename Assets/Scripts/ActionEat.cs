using UnityEngine;

public class ActionEat : Action
{
    private const int framesToEat = 10;

    private int framesElapsed = 0;
    private IA artificialIntelligence;
    private GameObject food;

    public ActionEat(IA artificialIntelligence, GameObject food)
    {
        this.artificialIntelligence = artificialIntelligence;
        this.food = food;
    }

    public override (bool done, bool result) Execute()
    {
        if (food == null) {
            return (true, false);
        }

        framesElapsed += 1;

        if (framesElapsed > framesToEat) {
            artificialIntelligence.hunger = 0;
            artificialIntelligence.currentState = IA.state.idle;
            GameObject.Destroy(food);
            return (true, true);
        }

        return (false, false);
    }

    public override bool CanBeExecuted()
    {
        if (food == null) {
            return false;
        }

        return Vector3.Distance(food.transform.position,
                artificialIntelligence.gameObject.transform.position) < 1;
    }

    public override void Cancel()
    {

    }
}
