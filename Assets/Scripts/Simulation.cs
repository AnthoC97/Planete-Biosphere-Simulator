using UnityEngine;
using MoonSharp.Interpreter;

public class Simulation : MonoBehaviour
{
    private const string entityTag = "SimulationEntity";

    private Planet planet;
    private PBSNoiseScript planetNoiseScript;

    public void Awake()
    {
        UserData.RegisterAssembly();

        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Transform>();
        UserData.RegisterType<Color>();
        UserData.RegisterType<GameObject>();
        UserData.RegisterType<LuaAPI>();

        UserData.RegisterType<Entity>();
        UserData.RegisterType<Simulation>();
        UserData.RegisterType<Time>();
        UserData.RegisterType<Input>();
        //UserData.RegisterType<PhysicsAPI>();
        UserData.RegisterType<Physics>();
        UserData.RegisterType<KeyCode>();
        UserData.RegisterType<UnityEngine.Random>();
        UserData.RegisterType<Collider>();
        UserData.RegisterType<MoveAction>();
        UserData.RegisterType<ActionScripted>();
        UserData.RegisterType<ActionGetAsleep>();
        UserData.RegisterType<ActionSleep>();
        UserData.RegisterType<ActionExecuteAfterDelay>();
        UserData.RegisterType<Script>();
        UserData.RegisterType<SharedContextProxy>();
    }

    private void Start()
    {
        GameObject rabbitPrefab = Resources.Load<GameObject>("Rabbit");

        planet = GameObject.Find("Planet").GetComponent<Planet>();
        planetNoiseScript =
            GameObject.Find("Planet").GetComponent<PBSNoiseScript>();

        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        GameObject[] water = GameObject.FindGameObjectsWithTag("Water");
        //GameObject rabbit = GameObject.Find("Rabbit");

        for (int i = 0; i < 100; ++i) {
            GameObject newFood = GameObject.Instantiate(food[0]);
            newFood.transform.position = Random.onUnitSphere;
        }

        for (int i = 0; i < 100; ++i) {
            GameObject newWater = GameObject.Instantiate(water[0]);
            newWater.transform.position = Random.onUnitSphere;
        }

        for (int i = 0; i < 50; ++i) {
            EntityFactory.AddEntityInWorld("rabbit", Random.onUnitSphere);
            //GameObject newRabbit = GameObject.Instantiate(rabbitPrefab);
            //newRabbit.transform.position = Random.onUnitSphere;
        }

        for (int i = 0; i < 10; ++i) {
            EntityFactory.AddEntityInWorld("fox", Random.onUnitSphere);
        }

        GameObject[] entities = GameObject.FindGameObjectsWithTag(entityTag);

        foreach (GameObject entity in entities) {
            Vector3 normalizedPosition = entity.transform.position.normalized;
            entity.transform.position =
                GetGroundPositionWithElevation(normalizedPosition, .5f);
        }

        food = GameObject.FindGameObjectsWithTag("Food");
        water = GameObject.FindGameObjectsWithTag("Water");

        foreach (GameObject entity in food) {
            Vector3 normalizedPosition = entity.transform.position.normalized;
            entity.transform.position =
                GetGroundPositionWithElevation(normalizedPosition, .5f);
        }

        foreach (GameObject entity in water) {
            Vector3 normalizedPosition = entity.transform.position.normalized;
            entity.transform.position =
                GetGroundPositionWithElevation(normalizedPosition, .5f);
        }
    }

    public Vector3 GetGroundPositionWithElevation(Vector3 normalizedPosition,
            float addedElevation)
    {
        float elevation = planetNoiseScript
            .GetNoiseGenerator().GetNoise3D(normalizedPosition);
        return normalizedPosition * (1 + elevation) * planet.radius;
    }

    public void FixedUpdate()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(entityTag);

        foreach (GameObject entity in entities) {
            Entity entityActions = entity.GetComponent<Entity>();

            if (entityActions == null) {
                continue;
            }

            if (entityActions.HasActionsQueued()) {
                var status = entityActions.ExecuteCurrentAction();
                if (status.done && !status.result
                        && status.action.OnFailureCallback != null) {
                    status.action.OnFailureCallback();
                }
            }
        }
    }
}
