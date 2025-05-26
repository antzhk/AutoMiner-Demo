using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private string prefab_key;
    
    [SerializeField] private SpawnerZone zone;
    
    [SerializeField] private float spawn_delay;
    
    [SerializeField] private int spawn_count;

    [SerializeField] private int maxSpots;

    [SerializeField] private List<ItemConfig> resourceConfigs;

    private float timer;

    public void Update()
    {
        timer += Time.deltaTime;
        
        if (timer < spawn_delay || ItemSpot.GetSpotsCount() >= maxSpots)
            return;

        timer = 0f;

        var resoucrceConfig = resourceConfigs[Random.Range(0, resourceConfigs.Count)];

        ItemSpot.CreateSpot(resoucrceConfig, zone.GetRandomSpawnPosition(), prefab_key);
    }
}
