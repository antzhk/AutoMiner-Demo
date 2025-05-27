using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemSpot : MonoBehaviour
{
    [SerializeField] private int minItemsCount;
    [SerializeField] private int maxItemsCount;
    
    public ItemConfig Config {get; private set;}
    
    private Miner currentMiner;

    private int itemCount;

    private static List<ItemSpot> itemsSpots = new List<ItemSpot>();

    public void Awake()
    {
        itemsSpots.Add(this);
    }

    public void OnDestroy()
    {
        itemsSpots.Remove(this);
    }
    
    public bool HasMiner()
    {
        return currentMiner != null;
    }

    public void SetMiner(Miner miner)
    {
        this.currentMiner = miner;
    }

    public void RemoveMiner()
    {
        this.currentMiner = null;
    }

    public int Harvest(int count)
    {
        var returnedCount = 0;

        if (this.itemCount >= count)
        {
            this.itemCount -= count;
            returnedCount = count;
        }
        else
        {
            returnedCount = this.itemCount;
            this.itemCount = 0;
            
            Destroy(this.gameObject);
        }

        return returnedCount;
    }
    
    public static ItemSpot CreateSpot(ItemConfig config, Vector3 position, string spot)
    {
        var prefab = Addressables.LoadAssetAsync<GameObject>(spot).WaitForCompletion();

        var obj = Instantiate(prefab, position, Quaternion.identity);

        var itemSpot = obj.GetComponent<ItemSpot>();
        
        itemSpot.SetItem(config);

        return itemSpot;
    }

    public static List<ItemSpot> GetAllSpots() => itemsSpots;

    public static int GetSpotsCount()
    {
        return itemsSpots.Count;
    }

    private void SetItem(ItemConfig config)
    {
        this.Config = config;

        this.itemCount = Random.Range(minItemsCount, maxItemsCount + 1);
    }
}