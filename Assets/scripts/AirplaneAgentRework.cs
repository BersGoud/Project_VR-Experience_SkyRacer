using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using HeneGames.Airplane;

public class AirplaneAgentRework : Agent
{
    public float speed = 10f;
    public float rotationSpeed = 100f;
    public float verticalSpeed = 5f; // New variable for vertical speed
    private Rigidbody rb;
    public bool threedimentional = false;

    public CheckpointTrainer CheckpointTrainer;
    private Transform nextCheckpoint;
    private float previousDistanceToCheckpoint;

    [Header("Engine propellers settings")]
    [Range(10f, 10000f)]

    [SerializeField] private GameObject[] propellers;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();

        if (CheckpointTrainer == null)
        {
            Debug.LogError("CheckpointTrainer not assigned!");
        }

        nextCheckpoint = CheckpointTrainer?.GetNextCheckpoint();
        if (nextCheckpoint != null)
        {
            previousDistanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
        }

    }

    public override void OnEpisodeBegin()
    {
        // Reset the airplane's position and rotation at the start of each episode
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Generate random positions within the specified range
        float randomX = Random.Range(-50, 50);
        float randomY = 0;
        if (threedimentional)
        {
            randomY = Random.Range(0, 10);
        }


        float randomZ = Random.Range(-50, 50);

        transform.localPosition = new Vector3(randomX, randomY, randomZ);

        // Generate slight random deviations in orientation
        float randomRotationX = 0;
        float randomRotationY = Random.Range(-180f, 180f);
        float randomRotationZ = 0;

        transform.localRotation = Quaternion.Euler(randomRotationX, randomRotationY, randomRotationZ);

        // Reset checkpoints
        CheckpointTrainer.ResetCheckpoints();
        nextCheckpoint = CheckpointTrainer?.GetNextCheckpoint();
        if (nextCheckpoint != null)
        {
            previousDistanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
        }
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
        float moveVertical = threedimentional ? actionBuffers.ContinuousActions[2] : 0f;

        // Apply forward movement
        Vector3 forwardMove = transform.forward * moveForward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMove);

        // Apply horizontal rotation
        float rotation = moveLeftRight * rotationSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        // Apply vertical movement if 3D movement is enabled
        if (threedimentional)
        {
            Vector3 verticalMove = transform.up * moveVertical * verticalSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + verticalMove);
        }

        // Reward for reducing distance to the checkpoint
        if (nextCheckpoint != null)
        {
            float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            float distanceDelta = previousDistanceToCheckpoint - distanceToCheckpoint;

            AddReward(distanceDelta * 0.1f); // Reward proportional to the distance reduced
            previousDistanceToCheckpoint = distanceToCheckpoint;

            // Reward for reaching the same altitude as the checkpoint
            // float altitudeDifference = Mathf.Abs(transform.position.y - nextCheckpoint.position.y);
            // float altitudeReward = Mathf.Max(0, 1f - (altitudeDifference / 10f)); // Reward decreases as altitude difference increases
            // AddReward(altitudeReward * 0.05f);
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

        if (threedimentional)
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
        if (other.CompareTag("Checkpoint"))
        {
            AddReward(1f);
            Debug.Log("Checkpoint reached");
            CheckpointTrainer.ReachedCheckpoint();
            nextCheckpoint = CheckpointTrainer.GetNextCheckpoint();
            if (nextCheckpoint != null)
            {
                previousDistanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            }
            EndEpisode(); // End the episode when a checkpoint is reached
        }
        else if (other.CompareTag("Wall"))
        {
            AddReward(-1f);
            Debug.Log("Collided with wall");
            CheckpointTrainer.ReachedCheckpoint();
            nextCheckpoint = CheckpointTrainer.GetNextCheckpoint();
            if (nextCheckpoint != null)
            {
                previousDistanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            }
            EndEpisode();
        }
    }

    private void RotatePropellers(GameObject[] _rotateThese, float _speed)
    {
        for (int i = 0; i < _rotateThese.Length; i++)
        {
            _rotateThese[i].transform.Rotate(Vector3.forward * -_speed * Time.deltaTime);
        }
    }

    private void UpdatePropellers()
    {
        //Rotate propellers if any
        if (propellers.Length > 0)
        {
            RotatePropellers(propellers, 1000);
        }
    }

    private void Update()
    {
        UpdatePropellers();
    }
}
