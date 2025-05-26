using UnityEngine;

public class SpawnerZone : MonoBehaviour
{
    [SerializeField] private float zoneRadius;

    public Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * zoneRadius;
        
        return new Vector3(randomCircle.x, randomCircle.y, 0f);
    }
}