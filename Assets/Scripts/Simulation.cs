using UnityEngine;

public class Simulation : MonoBehaviour
{
    private const string entityTag = "SimulationEntity";

    private void Start()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(entityTag);

        foreach (GameObject entity in entities) {
            Entity entityActions = entity.GetComponent<Entity>();

            if (entityActions != null && !entityActions.HasActionsQueued()) {
                Vector3 position = entity.transform.position;
                position.x += 10;
                entityActions.AddAction(new MoveAction(position, entity));
            }
        }
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
