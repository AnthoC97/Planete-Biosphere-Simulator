require "state";

--public Text stateDisplay;
--public Slider hungerJauge;
--public Slider thirstJauge;

-- Start is called before the first frame update
function start()
    localSharedContext = sharedContext.GetEditable();

    localSharedContext["hunger"] = 0;
    localSharedContext["thirst"] = 0;
    localSharedContext["stamina"] = 100;
    localSharedContext["age"] = 2;
    localSharedContext["lifespawn"] = 6;
    localSharedContext["currentState"] = state.idle;

    sharedContext.Synchronize(localSharedContext);

    --sharedContext.hunger = 0;
    --sharedContext.thirst = 0;
    --sharedContext.stamina = 100;
    --sharedContext.age = 2;
    --sharedContext.lifespawn = 6;
    --sharedContext.currentState = state.idle;
end

-- Update is called once per frame
function update()
    localSharedContext = sharedContext.GetEditable();

    stats();

    sharedContext.Synchronize(localSharedContext);

    --hungerJauge.value = 100 - hunger;
    --thirstJauge.value = 100 - thirst;
    --stateDisplay.text = currentState.ToString();
end

function stats()
    localSharedContext["hunger"] = localSharedContext["hunger"] + 0.2 * Time.deltaTime;
    localSharedContext["thirst"] = localSharedContext["thirst"] + 0.5 * Time.deltaTime;
    localSharedContext["age"] = localSharedContext["age"] + 1 / 365 * Time.deltaTime;
    localSharedContext["stamina"] = localSharedContext["stamina"] - 1 * Time.deltaTime;
    check();

    if (Input.GetKeyDown(KeyCode.KeypadPlus)) then
        Time.timeScale = Time.timeScale * 2;
    end
    if (Input.GetKeyDown(KeyCode.KeypadMinus)) then
        Time.timeScale = Time.timeScale / 2;
    end
end

function check()
    if (localSharedContext["hunger"] >= 100 or localSharedContext["thirst"] >= 100
        or localSharedContext["stamina"] <= 0) then
        destroy(gameObject);
        return;
    end

    --if (currentState == state.idle) -- TODO If this doesn't work, check HasActionsQueued
    if (not entity.HasActionsQueued()) then
        if (localSharedContext["stamina"] < 30 and localSharedContext["hunger"] < 80
            and localSharedContext["thirst"] < 75) then
            localSharedContext["currentState"] = state.going_to_sleep;
        elseif (localSharedContext["stamina"] < 5) then
            --currentState = state.sleeping;
            localSharedContext["currentState"] = state.going_to_sleep;
        elseif (localSharedContext["thirst"] > 20) then
            localSharedContext["currentState"] = state.searching_for_water;
        elseif (localSharedContext["hunger"] > localSharedContext["thirst"]
            and localSharedContext["hunger"] > 20) then
            localSharedContext["currentState"] = state.searching_for_food;
        end
    end

    stateBehaviour();
end

function stateBehaviour()
    if localSharedContext["currentState"] == state.searching_for_water then
        water = firstWithTagInSenseRange("Water");
        if water ~= nil then
            moveTowards(water);
            drinkWater(water);
            localSharedContext["currentState"] = state.drinking;
        else
            if (not entity.HasActionsQueued()) then
                wanderAround();
            end
        end
    elseif localSharedContext["currentState"] == state.searching_for_food then
        food = firstWithTagInSenseRange("Food");
        if (food ~= nil) then
            moveTowards(food);
            eatFood(food);
            localSharedContext["currentState"] = state.feeding;
        else
            if (not entity.HasActionsQueued()) then
                wanderAround();
            end
        end
    elseif localSharedContext["currentState"] == state.going_to_sleep then
        --entity.AddAction(ActionGetAsleep.__new(this));
        local action = ActionExecuteAfterDelay.__new(10, "putToSleep", this);
        entity.AddAction(action);
        --entity.AddAction(ActionSleep.__new(this));
        action = ActionScripted.__new("sleep.lua", gameObject);
        action.SetGlobal("sharedContext", sharedContext);
        entity.AddAction(action);
        --sharedContext["currentState"] = state.sleeping;
    end
end

function putToSleep()
    local editableContext = sharedContext.GetEditable();
    editableContext["currentState"] = state.sleeping;
    sharedContext.Synchronize(editableContext);
end

function firstWithTagInSenseRange(tag)
    colliders = Physics.OverlapSphere(gameObject.transform.position, 10, 1);

    for i = 1, #colliders do
        collided = colliders[i].gameObject;
        if (collided.name ~= gameObject.name and collided.CompareTag(tag)) then
            print(colliders[i].gameObject.transform.name);
            return colliders[i].gameObject;
        end
    end

    return nil;
end

function moveTowards(target)
    moveAction = MoveAction.__new(target.transform.position, gameObject, .2);
    entity.AddAction(moveAction);
end

function drinkWater(water)
    --entity.AddAction(new ActionDrink(this, water));
    action = ActionScripted.__new("drink.lua", gameObject);
    action.SetGlobal("artificialIntelligence", sharedContext);
    action.SetGlobal("water", water);
    entity.AddAction(action);
end

function eatFood(food)
    --entity.AddAction(ActionEat.__new(this, food));
    action = ActionScripted.__new("eat.lua", gameObject);
    action.SetGlobal("artificialIntelligence", sharedContext);
    action.SetGlobal("food", food);
    entity.AddAction(action);
end

function wanderAround()
    speed = .2;

    --Vector3 movement = gameObject.transform.forward * speed;
    --Vector3 destination = gameObject.transform.position + movement;

    angle = 1;

    actualPosition = gameObject.transform.position;
    --actualRotation = gameObject.transform.rotation;

    transform = gameObject.transform;

    rand = Random.value*2-1;
    turningRate = 45;
    transform.Rotate(Vector3.__new(0, rand * turningRate, 0));

    transform.RotateAround(Vector3.__new(0, 0, 0), transform.right, angle);
    normalizedPosition = transform.position.normalized;
    groundPosition =
    simulation.GetGroundPositionWithElevation(normalizedPosition, .5);

    entity.AddAction(MoveAction.__new(groundPosition, gameObject, speed));

    gameObject.transform.position = actualPosition;
    --gameObject.transform.rotation = actualRotation;
end
