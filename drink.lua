framesToDrink = 10;
local framesElapsed = 0;

function execute()
    if water == nil then
        print("water is nil!");
        return true, false;
    end

    framesElapsed = framesElapsed + 1;

    if framesElapsed > framesToDrink then
        artificialIntelligence.thirst = 0;
        artificialIntelligence.currentState = state.idle;
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
    artificialIntelligence.gameObject.transform.position) < 1;
end
