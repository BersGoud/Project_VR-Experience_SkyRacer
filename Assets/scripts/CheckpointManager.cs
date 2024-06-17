using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Transform> checkpoints;
    private int currentCheckpointIndex = 0;
    private int currentCheckpointIndexAgent = 0;


    public Transform GetNextCheckpoint()
    {
        if (currentCheckpointIndex < checkpoints.Count)
        {
            return checkpoints[currentCheckpointIndex];
        }
        return null;
    }

    public void ReachedCheckpoint()
    {
        currentCheckpointIndex++;
    }

    public void ResetCheckpoints()
    {
        currentCheckpointIndex = 0;
    }

    public Transform GetNextCheckpointAgent()
    {
        if (currentCheckpointIndexAgent < checkpoints.Count)
        {
            return checkpoints[currentCheckpointIndexAgent];
        }
        return null;
    }

    public void ReachedCheckpointAgent()
    {
        currentCheckpointIndexAgent++;
    }

    public void ResetCheckpointsAgent()
    {
        currentCheckpointIndexAgent = 0;
    }
}