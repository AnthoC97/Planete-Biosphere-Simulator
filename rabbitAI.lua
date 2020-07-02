state = {
    wandering = 0,
    drinking = 1,
    feeding = 2,
    searching_for_food = 3,
    searching_for_water = 4,
    going_to_sleep = 5,
    sleeping = 6,
    running = 7,
    hiding = 8,
    idle = 9,
    hunting = 10,
    dying = 11
};

local hunger = 0;
local thirst = 0;
local stamina = 100;
local age = 2;
local lifespawn = 6;
local currentState = state.idle;

--public Text stateDisplay;
--public Slider hungerJauge;
--public Slider thirstJauge;

-- Start is called before the first frame update
function start()
end

-- Update is called once per frame
function update()
    stats();
    --hungerJauge.value = 100 - hunger;
    --thirstJauge.value = 100 - thirst;
    --stateDisplay.text = currentState.ToString();
end

function stats()
    hunger = hunger + 0.2 * Time.deltaTime;
    thirst = thirst + 0.5 * Time.deltaTime;
    age = age + 1 / 365 * Time.deltaTime;
    stamina = stamina - 1 * Time.deltaTime;
    check();

    if (Input.GetKeyDown(KeyCode.KeypadPlus)) then
        Time.timeScale = Time.timeScale * 2;
    end
    if (Input.GetKeyDown(KeyCode.KeypadMinus)) then
        Time.timeScale = Time.timeScale / 2;
    end
end

function check()
    if (hunger >= 100 or thirst >= 100 or stamina <= 0) then
        GameObject.Destroy(gameObject);
        return;
    end

    --if (currentState == state.idle) -- TODO If this doesn't work, check HasActionsQueued
    if (not entity.HasActionsQueued()) then
        if (stamina < 30 and hunger < 80 and thirst < 75) then
            currentState = state.going_to_sleep;
        elseif (stamina < 5) then
            --currentState = state.sleeping;
            currentState = state.going_to_sleep;
        elseif (thirst > 20) then
            currentState = state.searching_for_water;
        elseif (hunger > thirst and hunger > 20) then
            currentState = state.searching_for_food;
        end
    end

    stateBehaviour();
end

function stateBehaviour()
    if currentState == state.searching_for_water then
        water = firstWithTagInSenseRange("Water");
        if water ~= nil then
            moveTowards(water);
            drinkWater(water);
            currentState = state.drinking;
        else
            if (not entity.HasActionsQueued()) then
                wanderAround();
            end
        end
    elseif currentState == state.searching_for_food then
        food = firstWithTagInSenseRange("Food");
        if (food ~= nil) then
            moveTowards(food);
            eatFood(food);
            currentState = state.feeding;
        else
            if (not entity.HasActionsQueued()) then
                wanderAround();
            end
        end
    elseif currentState == state.going_to_sleep then
        entity.AddAction(ActionGetAsleep.__new(this));
        entity.AddAction(ActionSleep.__new(this));
        currentState = state.sleeping;
    end
end

function firstWithTagInSenseRange(tag)
    colliders = Physics.OverlapSphere(gameObject.transform.position, 10, 1);

    print("#colliders: " .. #colliders);
    for i = 0, #colliders do
        print("type(colliders[" .. i .. "]): " .. type(colliders[i]));
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
    action = new ActionScripted.__new("drink.lua", gameObject);
    converterScript = Script.__new();
    action.SetGlobal("artificialIntelligence",
    DynValue.FromObject(converterScript, this));
    action.SetGlobal("water", DynValue.FromObject(converterScript, water));
    action.SetGlobal("state", state);
    entity.AddAction(action);
end

function eatFood(food)
    entity.AddAction(ActionEat.__new(this, food));
end

function WanderAround()
    speed = .2;

    --Vector3 movement = gameObject.transform.forward * speed;
    --Vector3 destination = gameObject.transform.position + movement;

    angle = 1;

    actualPosition = gameObject.transform.position;
    actualRotation = gameObject.transform.rotation;

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
