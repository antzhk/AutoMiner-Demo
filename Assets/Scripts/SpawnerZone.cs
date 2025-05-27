using UnityEngine;

public class SpawnerZone : MonoBehaviour
{
    [SerializeField] private float zoneRadius;

    public Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * zoneRadius;

        return transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);
    }
    
    public Vector3 GetSafeSpawnPosition(float radius = 0.5f, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPos = GetRandomSpawnPosition(); 
            bool isOverlapping = Physics2D.OverlapCircle(randomPos, radius, ~0);

            if (!isOverlapping)
                return randomPos;
        }
        
        return GetRandomSpawnPosition();
    }
}