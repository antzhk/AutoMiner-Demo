using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Base : MonoBehaviour
{
    [SerializeField] private string baseId;
    [SerializeField] private TextMeshProUGUI textView;
    [SerializeField] private int minerCount;
    [SerializeField] private int minersSpeed;
    [SerializeField] private string miner_key;
    
    [SerializeField] private float transferItemsDelay;
    
    [SerializeField] private SpawnerZone spawnZone;
    
    private static List<Base> allBases = new List<Base>();
    
    private Dictionary<string, int> itemsMap = new Dictionary<string, int>();

    public void Awake()
    {
        allBases.Add(this);
    }
    
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

    public void ChangeMinerSpeed(int speed)
    {
        foreach (var miner in Miner.GetAllBaseMiners(this.baseId))
        {
            miner.SetSpeed(speed);
        }

        this.minersSpeed = speed;
    }

    public int GetMinersSpeed()
    {
        return this.minersSpeed;
    }

    public void OnDestroy()
    {
        allBases.Remove(this);
    }

    public float GetTransferTime()
    {
        return this.transferItemsDelay;
    }

    public void TransferResources(string itemId, int itemCount)
    {
        if (!this.itemsMap.TryAdd(itemId, itemCount))
        {
            this.itemsMap[itemId] += itemCount;
        }
        
        textView.text = this.itemsMap[itemId].ToString();
    }

    public int GetItemsCount(string itemId)
    {
        return this.itemsMap.GetValueOrDefault(itemId, 0);
    }

    public string GetId()
    {
        return this.baseId;
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

    private void SpawnMiners(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var miner = Miner.CreateMiner(this.baseId, miner_key, spawnZone.GetSafeSpawnPosition());
            
            miner.SetSpeed(minersSpeed);
        }

        this.minerCount = Miner.GetAllBaseMiners(this.baseId).Count;
    }

    private void RemoveMiners(int count)
    {
        var counter = 0;
        foreach (var miner in Miner.GetAllBaseMiners(this.baseId))
        {
            Destroy(miner.gameObject);
            counter++;

            if (counter == count)
            {
                this.minerCount -= count;
                return;
            }
        }
    }
}
