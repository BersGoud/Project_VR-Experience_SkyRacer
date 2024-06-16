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
        if (airplaneController == null)
        {
            Debug.LogError("SimpleAirPlaneController not found!");
        }

        checkpointManager = FindObjectOfType<CheckpointManager>();
        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not found!");
        }

        nextCheckpoint = checkpointManager?.GetNextCheckpoint();
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
        // sensor.AddObservation(airplaneController.TurboHeatValue());
        sensor.AddObservation(airplaneController.transform.localEulerAngles); // 3 values

        sensor.AddObservation(airplaneController.transform.forward); // 3 values
        sensor.AddObservation(airplaneController.transform.up); // 3 values

        // Add observation for the relative position to the next checkpoint
        if (nextCheckpoint != null)
        {
            Vector3 relativePosition = (nextCheckpoint.position - airplaneController.transform.position).normalized;
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
            continuousActions[1] // Vertical
        );

        if (nextCheckpoint != null)
        {
            Vector3 directionToCheckpoint = (nextCheckpoint.position - transform.position).normalized;
            float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);

            // Reward inversely proportional to the distance
            float distanceReward = 1.0f / (distanceToCheckpoint + 1.0f);

            // Reward for aligning with the direction to the checkpoint
            Vector3 forward = transform.forward;
            float directionReward = Vector3.Dot(forward, directionToCheckpoint);

            // Penalty for deviating from the path
            float deviationPenalty = Mathf.Abs(Vector3.Cross(forward, directionToCheckpoint).y);

            // Combine the rewards and penalties
            float totalReward = (distanceReward * 0.5f) + (directionReward * 0.3f) - (deviationPenalty * 0.2f);
            AddReward(totalReward);

            // Debug.Log($"Agent Position: {transform.position}, Next Checkpoint: {nextCheckpoint.position - airplaneController.transform.position}");
            // Debug.Log($"Distance to checkpoint: {distanceToCheckpoint}, Distance Reward: {distanceReward}, Direction Reward: {directionReward}, Deviation Penalty: {deviationPenalty}, Total Reward: {totalReward}");
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public void HandleCheckpointCollision(Collider other)
    {
        if (nextCheckpoint != null && other.transform == nextCheckpoint)
        {
            // Reward the agent for passing through the checkpoint
            AddReward(100f);
            Debug.Log("Points");
            //checkpointManager.ReachedCheckpoint();
            //nextCheckpoint = checkpointManager.GetNextCheckpoint();
            EndEpisode();
        }
    }

    public void WallColision()
    {
        Debug.Log("Collided with wall");
        AddReward(-30f);
        EndEpisode();
    }
}
