using System;
using UnityEngine;


public class MinersManager : MonoBehaviour
{
    [SerializeField] private float updateDelay = 1f;

    private float timer;
    
    public void Update()
    {
        this.timer += Time.deltaTime;

        if (this.timer < this.updateDelay)
            return;

        this.timer = 0f;
        
        AssignMinersToSpots();
    }
    
    private void AssignMinersToSpots()
    {
        foreach (var spot in ItemSpot.GetAllSpots())
        {
            if (spot.HasMiner())
                continue;

            Miner nearestMiner = FindNearestFreeMiner(spot.transform.position);
            if (nearestMiner != null)
            {
                nearestMiner.HarvestItem(spot);
            }
        }
    }

    private Miner FindNearestFreeMiner(Vector3 pos)
    {
        Miner nearest = null;
        float minDist = float.MaxValue;

        foreach (var miner in Miner.GetAllMiners())
        {
            if (miner.IsBusy())
                continue;

            float sqrDist = (miner.transform.position - pos).sqrMagnitude;
            
            if (sqrDist < minDist)
            {
                minDist = sqrDist;
                nearest = miner;
            }
        }

        return nearest;
    }
}