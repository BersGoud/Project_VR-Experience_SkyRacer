using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using HeneGames.Airplane;

public class AirplaneAgent : Agent
{
    private SimpleAirPlaneController airplaneController;
    private CheckpointManager checkpointManager;
    private Transform nextCheckpoint;

    public override void Initialize()
    {
        airplaneController = GetComponent<SimpleAirPlaneController>();
        checkpointManager = FindObjectOfType<CheckpointManager>();
        nextCheckpoint = checkpointManager.GetNextCheckpoint();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the airplane to initial state
        airplaneController.ResetAirplane();
        checkpointManager.ResetCheckpoints();
        nextCheckpoint = checkpointManager.GetNextCheckpoint();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add observations from the airplane controller
        sensor.AddObservation(airplaneController.CurrentSpeed());
        sensor.AddObservation(airplaneController.TurboHeatValue());
        sensor.AddObservation(airplaneController.transform.localEulerAngles);

        // Add observation for the relative position to the next checkpoint
        if (nextCheckpoint != null)
        {
            Vector3 relativePosition = nextCheckpoint.position - airplaneController.transform.position;
            sensor.AddObservation(relativePosition);
            sensor.AddObservation(Vector3.Distance(airplaneController.transform.position, nextCheckpoint.position));
        }
        else
        {
            // No checkpoint available, add a zero vector and zero distance
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;

        // Map the actions to the airplane controller inputs
        airplaneController.SetInputs(
            continuousActions[0], // Horizontal
            continuousActions[1], // Vertical
            continuousActions[2] > 0.5f, // Turbo
            continuousActions[3] > 0.5f, // Yaw Left
            continuousActions[4] > 0.5f  // Yaw Right
        );

        // Reward the agent for moving towards the next checkpoint
        if (nextCheckpoint != null)
        {
            float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            AddReward(1.0f / (distanceToCheckpoint + 1.0f)); // Reward inversely proportional to the distance
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.0f;
        continuousActionsOut[3] = Input.GetKey(KeyCode.Q) ? 1.0f : 0.0f;
        continuousActionsOut[4] = Input.GetKey(KeyCode.E) ? 1.0f : 0.0f;
    }

    public void HandleCheckpointCollision(Collider other)
    {
        if (nextCheckpoint != null && other.transform == nextCheckpoint)
        {
            // Reward the agent for passing through the checkpoint
            AddReward(1.0f);
            Debug.Log("Points");
            checkpointManager.ReachedCheckpoint();
            nextCheckpoint = checkpointManager.GetNextCheckpoint();
        }
    }
}
