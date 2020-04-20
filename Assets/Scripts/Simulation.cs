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

    public void FixedUpdate()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(entityTag);

        foreach (GameObject entity in entities) {
            Entity entityActions = entity.GetComponent<Entity>();

            if (entityActions != null && entityActions.HasActionsQueued()) {
                var status = entityActions.ExecuteCurrentAction();
                Debug.Log("Executing action, status: " + status.ToString());
            }
        }
    }
}
