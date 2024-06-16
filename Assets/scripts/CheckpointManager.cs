using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Transform> checkpoints;
    private int currentCheckpointIndex = 0;

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
}