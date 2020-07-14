require("state");

framesToEat = 10;
local framesElapsed = 0;

function execute()
    if food == nil then
        print("food is nil!");
        return true, false;
    end

    framesElapsed = framesElapsed + 1;

    if framesElapsed > framesToEat then
        local editableContext = artificialIntelligence.GetEditable();
        editableContext["hunger"] = 0;
        editableContext["currentState"] = state.idle;
        artificialIntelligence.Synchronize(editableContext);
        destroy(food);
        return true, true;
    end

    return false, false;
end

function canBeExecuted()
    if food == nil then
        return false;
    end

    return Vector3.Distance(food.transform.position,
                            actor.transform.position) < 1;
end
