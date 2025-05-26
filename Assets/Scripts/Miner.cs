using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Miner : MonoBehaviour
{
    [SerializeField] private AIPath pathFinder;
    
    [SerializeField] private int maxItems = 5; //max inventory capacity

    [SerializeField] private float unloadTime = 2f; // unload to base

    private ItemSpot currentTarget;
    
    private int itemsCount;
    
    private string currentItemId = "";
    
    private string baseId;

    private bool isHarvesting;
    
    private Transform baseTransform;

    private Coroutine harvestCoroutine;
    private Coroutine cancelCoroutine;

    private static List<Miner> allMiners = new List<Miner>();

    private void Awake()
    {
        allMiners.Add(this);
    }

    private void OnDestroy()
    {
        allMiners.Remove(this);
    }

    public static Miner CreateMiner(string baseId, string obj, Vector3 position)
    {
        var prefab = Addressables.LoadAssetAsync<GameObject>(obj).WaitForCompletion();
        var gameObject = Instantiate(prefab, position, Quaternion.identity);
        var miner = gameObject.GetComponent<Miner>();
        miner.SetBase(baseId);
        return miner;
    }

    private void SetBase(string id)
    {
        this.baseId = id;
        this.baseTransform = Base.GetBaseById(id).transform;
    }

    public bool CompareBase(string id) => this.baseId.Equals(id);

    public static List<Miner> GetAllBaseMiners(string baseId)
    {
        var miners = new List<Miner>();
        foreach (var miner in allMiners)
        {
            if (miner.CompareBase(baseId))
                miners.Add(miner);
        }
        return miners;
    }

    private void SetDestination(Vector3 position)
    {
        pathFinder.destination = position;
    }


    public void HarvestItem(ItemSpot itemSpot)
    {
        if (isHarvesting || itemSpot == null) return;
        
        if (cancelCoroutine != null)
        {
            StopCoroutine(cancelCoroutine);
            cancelCoroutine = null;
        }

        currentTarget = itemSpot;
        currentItemId = itemSpot.Config.itemId;
        currentTarget.SetMiner(this);

        if (harvestCoroutine != null)
            StopCoroutine(harvestCoroutine);

        harvestCoroutine = StartCoroutine(HarvestCycle());
    }
    
    private IEnumerator HarvestCycle()
    {
        isHarvesting = true;
        itemsCount = 0;

        SetDestination(currentTarget.transform.position);
        yield return new WaitUntil(() => !pathFinder.pathPending && !pathFinder.hasPath);

        float harvestSpeed = currentTarget.Config.harvestSpeed;

        while (itemsCount < maxItems)
        {
            int collected = currentTarget.Harvest(1);
            if (collected == 0)
                break;

            itemsCount += collected;
            yield return new WaitForSeconds(harvestSpeed);
        }

        SetDestination(baseTransform.position);
        yield return new WaitUntil(() => !pathFinder.pathPending && !pathFinder.hasPath);
        
        yield return UnloadItems(currentItemId);

        isHarvesting = false;
        harvestCoroutine = null;
        currentTarget = null;
        currentItemId = "";
    }
    
    private IEnumerator UnloadItems(string itemType)
    {
        if (itemsCount > 0)
        {
            yield return new WaitForSeconds(unloadTime);
            itemsCount = 0;
        }
    }
    
    public void CancelHarvest()
    {
        if (!isHarvesting && itemsCount == 0) return;
        
        if (harvestCoroutine != null)
        {
            StopCoroutine(harvestCoroutine);
            harvestCoroutine = null;
        }

        isHarvesting = false;

        if (currentTarget != null)
        {
            currentTarget.SetMiner(null);
            currentTarget = null;
            currentItemId = "";
        }
        
        if (itemsCount > 0)
        {
            if (cancelCoroutine != null)
                StopCoroutine(cancelCoroutine);

            cancelCoroutine = StartCoroutine(CancelReturnAndUnload());
        }
        else
        {
            SetDestination(baseTransform.position);
        }
    }
    
    private IEnumerator CancelReturnAndUnload()
    {
        SetDestination(baseTransform.position);

        yield return new WaitUntil(() => !pathFinder.pathPending && !pathFinder.hasPath);

        yield return UnloadItems(currentItemId);

        cancelCoroutine = null;
        itemsCount = 0;
    }
}
