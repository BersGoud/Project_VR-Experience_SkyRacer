using UnityEngine;

public class CheckpointTrainer : MonoBehaviour
{
    public GameObject checkpointPrefab;
    public Vector3 spawnArea = new Vector3(50, 10, 50);
    private Transform currentCheckpoint;
    public Transform agentTransform;

    private void Start()
    {
        SpawnCheckpoint();
    }

    public Transform GetNextCheckpoint()
    {
        return currentCheckpoint;
    }

    public void ReachedCheckpoint()
    {
        Destroy(currentCheckpoint.gameObject);
        SpawnCheckpoint();
    }

    public void ResetCheckpoints()
    {
        if (currentCheckpoint != null)
        {
            Destroy(currentCheckpoint.gameObject);
        }
        SpawnCheckpoint();
    }

    private void SpawnCheckpoint()
    {
        Vector3 localSpawnPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(0, 20),
            //0,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        // Transform the local spawn position to a world position
        Vector3 worldSpawnPosition = transform.TransformPoint(localSpawnPosition);

        GameObject newCheckpoint = Instantiate(checkpointPrefab, worldSpawnPosition, Quaternion.identity);
        currentCheckpoint = newCheckpoint.transform;
    }
}
