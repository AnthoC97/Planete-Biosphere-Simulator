require "state";

-- Start is called before the first frame update
function start()
    localSharedContext = sharedContext.GetEditable();

    localSharedContext["hunger"] = 0;
    localSharedContext["stamina"] = 100;
    localSharedContext["age"] = 2;
    localSharedContext["currentState"] = state.idle;

    sharedContext.Synchronize(localSharedContext);

    API.AddUIText(gameObject, "currentState", stateTranslation);
    --API.AddUISlider(gameObject, "thirst", 0, 100, 0, 203/255, 229/255);
    API.AddUISlider(gameObject, "hunger", 0, 100, 0, 1, 24/255);
    API.AddUISlider(gameObject, "stamina", 0, 100);
end

-- Update is called once per frame
function update()
    localSharedContext = sharedContext.GetEditable();

    stats();

    sharedContext.Synchronize(localSharedContext);
end

function stats()
    localSharedContext["hunger"] = localSharedContext["hunger"] + 0.8 * Time.deltaTime;
    localSharedContext["age"] = localSharedContext["age"] + 1 / 365 * Time.deltaTime;
    localSharedContext["stamina"] = localSharedContext["stamina"] - 1 * Time.deltaTime;
    check();
end

function check()
    if (localSharedContext["hunger"] >= 100
        or localSharedContext["stamina"] <= 0) then
        destroy(gameObject);
        return;
    end

    --if (currentState == state.idle) -- TODO If this doesn't work, check HasActionsQueued
    if (not entity.HasActionsQueued()) then
        if (localSharedContext["stamina"] < 30
            and localSharedContext["hunger"] < 80) then
            localSharedContext["currentState"] = state.going_to_sleep;
        elseif (localSharedContext["stamina"] < 5) then
            --currentState = state.sleeping;
            localSharedContext["currentState"] = state.going_to_sleep;
        elseif (localSharedContext["hunger"] > 20) then
            localSharedContext["currentState"] = state.searching_for_food;
        end
    end

    stateBehaviour();
end

function stateBehaviour()
    if localSharedContext["currentState"] == state.searching_for_food then
        food = firstWithNameInSenseRange("rabbit");
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

function searchForFood()
    local editableContext = sharedContext.GetEditable();
    editableContext["currentState"] = state.searching_for_food;
    sharedContext.Synchronize(editableContext);
end

function firstWithNameInSenseRange(name)
    colliders = Physics.OverlapSphere(gameObject.transform.position, 10, 1);

    for i = 1, #colliders do
        local collided = colliders[i].gameObject;
        if (collided.name == name) then
            --print(colliders[i].gameObject.transform.name);
             return colliders[i].gameObject;
        end
    end

    return nil;
end

function moveTowards(target)
    moveAction = MoveAction.__new(target.transform.position, gameObject, .4);
    entity.AddAction(moveAction);
end

function eatFood(food)
    --entity.AddAction(ActionEat.__new(this, food));
    action = ActionScripted.__new("eat.lua", gameObject);
    action.SetGlobal("artificialIntelligence", sharedContext);
    action.SetGlobal("food", food);
    action.SetGlobal("framesToEat", 0);
    action.OnFailure(searchForFood);
    entity.AddAction(action);
end

function wanderAround()
    speed = .3;

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
