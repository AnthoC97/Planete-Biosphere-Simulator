using UnityEngine;

public class Simulation : MonoBehaviour
{
    private const string entityTag = "SimulationEntity";

    private Planet planet;
    private PBSNoiseScript planetNoiseScript;

    private void Start()
    {
        planet = GameObject.Find("Planet").GetComponent<Planet>();
        planetNoiseScript = GameObject.Find("Planet").GetComponent<PBSNoiseScript>();
        GameObject[] entities = GameObject.FindGameObjectsWithTag(entityTag);

        foreach (GameObject entity in entities) {
            Vector3 normalizedPosition = entity.transform.position.normalized;
            entity.transform.position =
                GetGroundPositionWithElevation(normalizedPosition, .5f);
            Debug.Log(entity.transform.position);

            Entity entityActions = entity.GetComponent<Entity>();

            if (entityActions != null && !entityActions.HasActionsQueued()) {
                Vector3 position = entity.transform.position;
                Debug.Log("Old position: " + position.ToString());
                position.y += 10;
                position =
                    GetGroundPositionWithElevation(position.normalized, .5f);
                Debug.Log("Destination: " + position.ToString());
                entityActions.AddAction(new MoveAction(position, entity, .2f));
            }
        }
    }

    private Vector3 GetGroundPositionWithElevation(Vector3 normalizedPosition,
            float addedElevation)
    {
        float elevation = planetNoiseScript
            .GetNoiseGenerator().GetNoise3D(normalizedPosition);
        return normalizedPosition * (1 + elevation) * planet.radius;
    }

    private void MoveEntityRelative(GameObject entity, Entity entityActions,
            Vector3 movement)
    {
        Vector3 position = entity.transform.position;
        Debug.Log("Old position: " + position.ToString());
        position += movement;
        position = GetGroundPositionWithElevation(position.normalized, .5f);
        Debug.Log("Destination: " + position.ToString());
        entityActions.AddAction(new MoveAction(position, entity, .2f));
    }

    public void Update()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(entityTag);

        foreach (GameObject entity in entities) {
            Entity entityActions = entity.GetComponent<Entity>();

            if (entityActions == null || entityActions.HasActionsQueued()) {
                continue;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                Vector3 movement = new Vector3(10, 0, 0);
                MoveEntityRelative(entity, entityActions, movement);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                Vector3 movement = new Vector3(-10, 0, 0);
                MoveEntityRelative(entity, entityActions, movement);
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                Vector3 movement = new Vector3(0, 0, 10);
                MoveEntityRelative(entity, entityActions, movement);
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                Vector3 movement = new Vector3(0, 0, -10);
                MoveEntityRelative(entity, entityActions, movement);
            } else if (Input.GetKeyDown(KeyCode.PageUp)) {
                Vector3 movement = new Vector3(0, 10, 0);
                MoveEntityRelative(entity, entityActions, movement);
            } else if (Input.GetKeyDown(KeyCode.PageDown)) {
                Vector3 movement = new Vector3(0, -10, 0);
                MoveEntityRelative(entity, entityActions, movement);
            }
        }
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
                Debug.Log("Executing action, status: " + status.ToString());
            }
        }
    }
}
