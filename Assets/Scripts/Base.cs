using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private string baseId;
    [SerializeField] private int minerCount;
    [SerializeField] private float minersSpeed;
    [SerializeField] private string miner_key;
    [SerializeField] private SpawnerZone spawnZone;
    
    private static List<Base> allBases = new List<Base>();

    public void Start()
    {
        SpawnMiners(minerCount);
    }

    public void ChangeMinersCount(int count)
    {
        if (count == this.minerCount || count < 0) return;
        
        if (count < this.minerCount)
        {
            this.RemoveMiners(this.minerCount - count);
        }

        if (count > this.minerCount)
        {
            this.SpawnMiners(count - this.minerCount);
        }
    }

    public static Base GetBaseById(string id)
    {
        foreach (var hq in allBases)
        {
            if (hq.baseId.Equals(id))
                return hq;
        }

        return null;
    }
    
    public void Awake()
    {
        allBases.Add(this);
    }

    public void OnDestroy()
    {
        allBases.Remove(this);
    }

    private void SpawnMiners(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Miner.CreateMiner(this.baseId, miner_key, spawnZone.GetRandomSpawnPosition());
        }

        this.minerCount = Miner.GetAllBaseMiners(this.baseId).Count;
    }

    private void RemoveMiners(int count)
    {
        var destroyedCount = 0;
        
        foreach (var miner in Miner.GetAllBaseMiners(this.baseId))
        {
            Destroy(miner.gameObject);
            destroyedCount++;

            if (destroyedCount == count)
                return;
        }
    }
}
