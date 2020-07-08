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
        artificialIntelligence["hunger"] = 0;
        artificialIntelligence["currentState"] = state.idle;
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
