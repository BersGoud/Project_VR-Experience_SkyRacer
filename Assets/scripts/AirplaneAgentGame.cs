using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AirplaneAgentGame : Agent
{
    public float speed = 10f;
    public float rotationSpeed = 100f;
    public float verticalSpeed = 5f; // Variable for vertical speed
    private Rigidbody rb;
    public bool threedimensional = false;
    public Transform startposition;

    public CheckpointManager checkpointManager; // Changed to CheckpointManager
    private Transform nextCheckpoint;
    private float previousDistanceToCheckpoint;
    private float previousVerticalDistanceToCheckpoint;

    public GameObject propeller;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();

        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not assigned!");
        }

        checkpointManager.ResetCheckpointsAgent();
        nextCheckpoint = checkpointManager.GetNextCheckpointAgent();
    }


    public override void OnEpisodeBegin()
    {
        // Reset checkpoints
        checkpointManager.ResetCheckpointsAgent();
        nextCheckpoint = checkpointManager.GetNextCheckpointAgent();

        // Reset the airplane's velocity and angular velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset the airplane's orientation to be upright
        rb.rotation = Quaternion.identity;

        rb.position = startposition.transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Normalize and add rotation angles
        Vector3 normalizedRotation = transform.localRotation.eulerAngles / 360f;
        sensor.AddObservation(normalizedRotation);

        // Add observation for the relative position to the next checkpoint
        if (nextCheckpoint != null)
        {
            Vector3 relativePosition = (nextCheckpoint.position - transform.position) / 100f;
            sensor.AddObservation(relativePosition);
            sensor.AddObservation(Vector3.Distance(transform.position, nextCheckpoint.position) / 100f);
        }
        else
        {
            // No checkpoint available, add a zero vector and zero distance
            Debug.Log("AAAAAAAAAAAAAAAH");
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }

        // Normalize and add velocity
        sensor.AddObservation(rb.velocity / speed);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 3 for 3D movement
        float moveForward = Mathf.Clamp(actionBuffers.ContinuousActions[0], 0, 1); // Ensure no backward movement
        float moveLeftRight = actionBuffers.ContinuousActions[1];
        float moveVertical = threedimensional ? actionBuffers.ContinuousActions[2] : 0f;

        // Apply forward movement
        Vector3 forwardMove = transform.forward * moveForward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMove);

        // Apply horizontal rotation
        float rotation = moveLeftRight * rotationSpeed * Time.deltaTime;
        Quaternion newRotation = rb.rotation * Quaternion.Euler(0, rotation, 0);

        Vector3 eulerRotation = newRotation.eulerAngles;
        eulerRotation.z = 0;
        eulerRotation.x = 0;
        rb.MoveRotation(Quaternion.Euler(eulerRotation));

        // Apply vertical movement if 3D movement is enabled
        if (threedimensional)
        {
            Vector3 verticalMove = transform.up * moveVertical * verticalSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + verticalMove);
        }

        // Reward for reducing distance to the checkpoint
        if (nextCheckpoint != null)
        {
            float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            float verticalDistanceToCheckpoint = Mathf.Abs(nextCheckpoint.position.y - transform.position.y);
            float distanceDelta = previousDistanceToCheckpoint - distanceToCheckpoint;
            float verticalDistanceDelta = previousVerticalDistanceToCheckpoint - verticalDistanceToCheckpoint;

            AddReward(distanceDelta * 0.1f); // Reward proportional to the distance reduced
            AddReward(verticalDistanceDelta * 0.05f); // Reward proportional to the vertical distance reduced

            previousDistanceToCheckpoint = distanceToCheckpoint;
            previousVerticalDistanceToCheckpoint = verticalDistanceToCheckpoint;
        }

        // Penalty for stalling or no movement
        if (moveForward == 0)
        {
            AddReward(-0.01f);
        }

        // Penalty for erratic spinning
        if (Mathf.Abs(moveLeftRight) > 0.5f)
        {
            AddReward(-0.01f);
        }

        // Small penalty over time to encourage faster completion
        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1); // Forward only, no backward movement
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // Left/Right

        if (threedimensional)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                continuousActionsOut[2] = 1f; // Move up
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                continuousActionsOut[2] = -1f; // Move down
            }
            else
            {
                continuousActionsOut[2] = 0f; // No vertical movement
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") && Vector3.Distance(transform.position, nextCheckpoint.position) < 40)
        {
            AddReward(5f);
            GameManager.instance.AddAIScore();
            // Debug.Log("Checkpoint reached");
            checkpointManager.ReachedCheckpointAgent();
            nextCheckpoint = checkpointManager.GetNextCheckpointAgent();
            if (nextCheckpoint != null)
            {
                previousDistanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            }
            if (checkpointManager.GetNextCheckpointAgent() == null) // Check if the last checkpoint is reached
            {
                Debug.Log("Circuit complete");
                AddReward(60f);
                checkpointManager.ResetCheckpointsAgent();
                nextCheckpoint = checkpointManager.GetNextCheckpointAgent();

            }
        }
        else if (other.CompareTag("Wall"))
        {
            AddReward(-2f);
            Debug.Log("Collided with wall");
        }
        else if (other.CompareTag("Obstacle"))
        {
            AddReward(-2f);
            Debug.Log("Collided with obstacle");
        }
    }

    private void RotatePropellers(GameObject _rotateThese, float _speed)
    {

        _rotateThese.transform.Rotate(Vector3.forward * -_speed * Time.deltaTime);

    }

    private void UpdatePropellers()
    {
        //Rotate propellers if any
        if (propeller)
        {
            RotatePropellers(propeller, 1000);
        }
    }

    private void Update()
    {
        UpdatePropellers();
    }
}
