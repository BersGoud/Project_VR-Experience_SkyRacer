using UnityEngine;
using System.Collections.Generic;
using Unity.Barracuda;

public class CheckpointTrainer2 : MonoBehaviour
{
    public GameObject checkpointPrefab;
    public GameObject obstaclePrefab; // Prefab for the obstacles
    public Vector3 spawnArea = new Vector3(50, 10, 50);
    public int checkpointCount = 5; // Number of checkpoints in the circuit
    public int obstacleCount = 10; // Number of obstacles
    public Transform agentTransform;
    private List<Transform> checkpoints = new List<Transform>();
    private int currentCheckpointIndex = 0;

    private void Start()
    {
        if (checkpointCount > 0)
        {
            SpawnCheckpoints();
            SpawnObstacles();
        }
        else
        {
            Debug.LogError("Checkpoint count must be greater than 0.");
        }
    }

    public Transform GetNextCheckpoint()
    {
        if (checkpoints.Count == 0)
        {
            Debug.LogError("No checkpoints available.");
            return null;
        }

        return checkpoints[currentCheckpointIndex];
    }

    public void ReachedCheckpoint()
    {
        if (checkpoints.Count == 0)
        {
            Debug.LogError("No checkpoints available.");
            return;
        }

        currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
    }

    public void ResetCheckpoints()
    {
        foreach (Transform checkpoint in checkpoints)
        {
            if (checkpoint != null)
            {
                Destroy(checkpoint.gameObject);
            }
        }
        checkpoints.Clear();
        currentCheckpointIndex = 0;

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer != 3)
            {
                Destroy(obj);
            }
        }

        if (checkpointCount > 0)
        {
            SpawnCheckpoints();
            SpawnObstacles();
        }
    }

    private void SpawnCheckpoints()
    {
        float angleStep = 360f / checkpointCount;
        float radiusX = spawnArea.x / 2;
        float radiusZ = spawnArea.z / 2;

        for (int i = 0; i < checkpointCount; i++)
        {
            float angle = i * angleStep;
            float angleRad = Mathf.Deg2Rad * angle;

            float x = radiusX * Mathf.Cos(angleRad);
            float z = radiusZ * Mathf.Sin(angleRad);
            float y = Random.Range(0, 40); // Random height

            Vector3 localSpawnPosition = new Vector3(x, y, z);
            Vector3 worldSpawnPosition = transform.TransformPoint(localSpawnPosition);

            GameObject newCheckpoint = Instantiate(checkpointPrefab, worldSpawnPosition, Quaternion.identity);
            checkpoints.Add(newCheckpoint.transform);
        }
    }

    private void SpawnObstacles()
    {
        float radiusX = spawnArea.x / 2;
        float radiusZ = spawnArea.z / 2;

        for (int i = 0; i < obstacleCount; i++)
        {
            float x = Random.Range(-radiusX, radiusX);
            float z = Random.Range(-radiusZ, radiusZ);
            float y = Random.Range(0, 30); // Random height

            Vector3 localSpawnPosition = new Vector3(x, y, z);
            Vector3 worldSpawnPosition = transform.TransformPoint(localSpawnPosition);

            Instantiate(obstaclePrefab, worldSpawnPosition, Quaternion.identity);
        }
    }

    public bool AreCheckpointsAvailable()
    {
        return checkpoints.Count > 0;
    }

    public bool IsCurrentCheckpointLast()
    {
        if (checkpoints.Count == 0)
        {
            Debug.LogError("No checkpoints available.");
            return false;
        }

        return currentCheckpointIndex == checkpoints.Count - 1;
    }
}
