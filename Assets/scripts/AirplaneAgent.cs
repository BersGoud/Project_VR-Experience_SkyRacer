using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using HeneGames.Airplane;

public class AirplaneAgent : Agent
{
    private SimpleAirPlaneController airplaneController;

    public override void Initialize()
    {
        airplaneController = GetComponent<SimpleAirPlaneController>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the airplane to initial state
        airplaneController.ResetAirplane();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add observations from the airplane controller
        sensor.AddObservation(airplaneController.CurrentSpeed());
        sensor.AddObservation(airplaneController.TurboHeatValue());
        sensor.AddObservation(airplaneController.transform.localEulerAngles);
        sensor.AddObservation(airplaneController.transform.position);
        // Add more observations as needed
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
}
