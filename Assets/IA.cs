using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IA : MonoBehaviour
{

    public float hunger, thirst, stamina, age, lifespawn;
    public enum state { wandering, drinking, feeding, searching_for_food, searching_for_water, going_to_sleep, sleeping, running, hiding, idle, hunting, dying};
    public state currentState;

    public Text stateDisplay;
    public Slider hungerJauge;
    public Slider thirstJauge;

    private Entity entity;
    private Simulation simulation;

    // Start is called before the first frame update
    void Start()
    {
        hunger = 0;
        thirst = 0;
        stamina = 100;
        age = 2;
        lifespawn = 6;
        currentState = state.idle;

        entity = this.gameObject.GetComponent<Entity>();
        simulation = GameObject.Find("Simulation").GetComponent<Simulation>();
    }

    // Update is called once per frame
    void Update()
    {
        stats();
        hungerJauge.value = 100 - hunger;
        thirstJauge.value = 100 - thirst;
        stateDisplay.text = currentState.ToString();
    }

    void stats()
    {
        hunger += 0.2f * Time.deltaTime;
        thirst += 0.5f * Time.deltaTime;
        age += 1 / 365 * Time.deltaTime;
        stamina -= 1f * Time.deltaTime;
        check();



        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            Time.timeScale = Time.timeScale * 2;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            Time.timeScale = Time.timeScale / 2;
    }

    void check()
    {
        //if (currentState == state.idle) // TODO If this doesn't work, check HasActionsQueued
        if (!entity.HasActionsQueued())
        {
            if (stamina < 30 && hunger < 80 &&  thirst < 75)
            {
                currentState = state.going_to_sleep;
            }
            else if (stamina < 5)
            {
                //currentState = state.sleeping;
                currentState = state.going_to_sleep;
            }
            else if (thirst > 20)
            {
                currentState = state.searching_for_water;
            }
            else if (hunger > thirst && hunger > 20f)
            {
                currentState = state.searching_for_food;
            }
            else if (hunger == 1000 || thirst == 1000 || stamina <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        }

        stateBehaviour();
    }

    void stateBehaviour()
    {
        switch (currentState)
        {
            case state.searching_for_water:
                GameObject water = FirstWithTagInSenseRange("Water");
                if (water != null) {
                    MoveTowards(water);
                    DrinkWater(water);
                    currentState = state.drinking;
                } else {
                    if (!entity.HasActionsQueued()) {
                        WanderAround();
                    }
                }
                break;
            case state.searching_for_food:
                GameObject food = FirstWithTagInSenseRange("Food");
                if (food != null) {
                    MoveTowards(food);
                    EatFood(food);
                    currentState = state.feeding;
                } else {
                    if (!entity.HasActionsQueued()) {
                        WanderAround();
                    }
                }
                break;
            case state.going_to_sleep:
                entity.AddAction(new ActionGetAsleep(this));
                entity.AddAction(new ActionSleep(this));
                currentState = state.sleeping;
                break;
            default:
                break;
        }
    }

    GameObject FirstWithTagInSenseRange(string tag)
    {
        Collider[] colliders =
            Physics.OverlapSphere(transform.position, 100, 1);

        for(int i = 0; i < colliders.Length; i++)
        {
            GameObject collided = colliders[i].gameObject;
            if (collided.name != gameObject.name && collided.CompareTag(tag)) {
                Debug.Log(colliders[i].gameObject.transform.name);
                return colliders[i].gameObject;
            }
        }

        return null;
    }

    void MoveTowards(GameObject target)
    {
        MoveAction moveAction =
            new MoveAction(target.transform.position, gameObject, .2f);
        entity.AddAction(moveAction);
    }

    void DrinkWater(GameObject water)
    {
        entity.AddAction(new ActionDrink(this, water));
    }

    void EatFood(GameObject food)
    {
        entity.AddAction(new ActionEat(this, food));
    }

    void WanderAround()
    {
        float speed = .2f;

        //Vector3 movement = gameObject.transform.forward * speed;
        //Vector3 destination = gameObject.transform.position + movement;

        float angle = 1f;

        Vector3 actualPosition = gameObject.transform.position;
        var actualRotation = gameObject.transform.rotation;

        Transform transform = gameObject.transform;

        float rand = Random.value*2-1;
        float turningRate = 45;
        transform.Rotate(new Vector3(0, rand * turningRate, 0));

        transform.RotateAround(new Vector3(0, 0, 0), transform.right, angle);
        Vector3 normalizedPosition = transform.position.normalized;
        Vector3 groundPosition =
            simulation.GetGroundPositionWithElevation(normalizedPosition, .5f);

        entity.AddAction(new MoveAction(groundPosition, gameObject, speed));

        gameObject.transform.position = actualPosition;
        //gameObject.transform.rotation = actualRotation;
    }
}
