require("state");

framesToDrink = 10;
local framesElapsed = 0;

function execute()
    if water == nil then
        print("water is nil!");
        return true, false;
    end

    framesElapsed = framesElapsed + 1;

    if framesElapsed > framesToDrink then
        local editableContext = artificialIntelligence.GetEditable();
        editableContext["thirst"] = 0;
        editableContext["currentState"] = state.idle;
        artificialIntelligence.Synchronize(editableContext);
        destroy(water);
        return true, true;
    end

    return false, false;
end

function canBeExecuted()
    if water == nil then
        return false;
    end

    return Vector3.Distance(water.transform.position,
                            actor.transform.position) < 1;
end
