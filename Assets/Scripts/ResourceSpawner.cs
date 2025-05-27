using System;
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

    private static ResourceSpawner instance;


    public void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        
        if (timer < spawn_delay || ItemSpot.GetSpotsCount() >= maxSpots)
            return;

        timer = 0f;

        for (int i = 0; i < spawn_count; i++)
        {
            var resoucrceConfig = resourceConfigs[Random.Range(0, resourceConfigs.Count)];

            ItemSpot.CreateSpot(resoucrceConfig, zone.GetSafeSpawnPosition(), prefab_key);
        }
        
        AstarPath.active.Scan();
    }

    public void ChangeDelay(float value)
    {
        this.spawn_delay = value;
    }

    public static ResourceSpawner Get()
    {
        return instance;
    }
}
