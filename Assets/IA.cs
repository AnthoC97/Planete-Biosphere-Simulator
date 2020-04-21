using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IA : MonoBehaviour
{

    float hunger, thirst, stamina, age, lifespawn;
    enum state { wandering, drinking, feeding, searching_for_food, searching_for_water, going_to_sleep, sleeping, running, hiding, idle, hunting, dying};
    state currentState;

    public Text stateDisplay;
    public Slider hungerJauge;
    public Slider thirstJauge;

    // Start is called before the first frame update
    void Start()
    {
        hunger = 0;
        thirst = 0;
        stamina = 100;
        age = 2;
        lifespawn = 6;
        currentState = state.idle;   
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
        check();



        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            Time.timeScale = Time.timeScale * 2;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            Time.timeScale = Time.timeScale / 2;
    }

    void check()
    {
        if (stamina < 30 && hunger < 80 &&  thirst < 75)
        {
            currentState = state.going_to_sleep;
        }
        else if (stamina < 5)
        {
            currentState = state.sleeping;
        }
        else if (thirst > 20)
        {
            currentState = state.searching_for_water;
        }
        else if (hunger > thirst && hunger > 20f)
        {
            currentState = state.searching_for_food;
        }
        else 
        {
            currentState = state.idle;
        }
        stateBehaviour();
    }

    void stateBehaviour()
    {
        switch (currentState)
        {
            case state.searching_for_water:

                break;
            default:
                break;
        }
    }
}
