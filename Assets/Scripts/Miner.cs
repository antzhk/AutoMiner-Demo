using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Miner : MonoBehaviour
{
    [SerializeField] private AIPath pathFinder;
    [SerializeField] private int maxItems = 5;

    private ItemSpot currentTarget;
    private int itemsCount;
    private string currentItemId = "";
    private string baseId;

    private bool isHarvesting;
    private Transform baseTransform;

    private Coroutine harvestCoroutine;
    private Coroutine cancelCoroutine;

    private static List<Miner> allMiners = new List<Miner>();

    public void Awake() => allMiners.Add(this);
    public void OnDestroy() => allMiners.Remove(this);

    public void SetSpeed(float speed) => pathFinder.maxSpeed = speed;
    public bool CompareBase(string id) => baseId.Equals(id);
    public bool IsBusy() =>  isHarvesting || itemsCount > 0;

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
            MoveTo(baseTransform.position);
        }
    }

    public static Miner CreateMiner(string baseId, string obj, Vector3 position)
    {
        var prefab = Addressables.LoadAssetAsync<GameObject>(obj).WaitForCompletion();
        var gameObject = Instantiate(prefab, position, Quaternion.identity);
        var miner = gameObject.GetComponent<Miner>();
        miner.SetBase(baseId);
        return miner;
    }

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

    public static List<Miner> GetAllMiners() => allMiners;

    private void SetBase(string id)
    {
        baseId = id;
        baseTransform = Base.GetBaseById(id).transform;
    }

    private void MoveTo(Vector3 position)
    {
        if ((pathFinder.destination - position).sqrMagnitude > 0.01f)
        {
            pathFinder.canSearch = true;
            pathFinder.canMove = true;
            pathFinder.destination = position;
        }
    }

    private void StopMovement()
    {
        pathFinder.destination = pathFinder.position;
        pathFinder.canSearch = false;
        pathFinder.canMove = false;
    }

    private IEnumerator HarvestCycle()
    {
        isHarvesting = true;
        itemsCount = 0;

        if (currentTarget == null)
        {
            Debug.LogWarning("HarvestCycle aborted: currentTarget is null.");
            isHarvesting = false;
            yield break;
        }

        MoveTo(currentTarget.transform.position);
        yield return WaitUntilDestinationReached();
        
        if (currentTarget == null)
        {
            Debug.LogWarning("Target destroyed before harvesting.");
            isHarvesting = false;
            yield break;
        }

        float harvestSpeed = currentTarget.Config.harvestSpeed;

        while (itemsCount < maxItems && currentTarget != null)
        {
            int collected = currentTarget.Harvest(1);
            if (collected == 0)
                break;

            itemsCount += collected;
            yield return new WaitForSeconds(harvestSpeed);
        }

        if (currentTarget != null)
            currentTarget.RemoveMiner();

        MoveTo(baseTransform.position);
        yield return WaitUntilDestinationReached();

        yield return UnloadItems(currentItemId);

        StopMovement();

        isHarvesting = false;
        harvestCoroutine = null;
        currentTarget = null;
        this.itemsCount = 0;
        currentItemId = "";
    }

    private IEnumerator WaitUntilDestinationReached(float timeout = 10f, float startDelay = 0.2f)
    {
        float timer = 0f;

        yield return new WaitForSeconds(startDelay);

        while (timer < timeout)
        {
            if (!pathFinder.pathPending && pathFinder.velocity.sqrMagnitude < 0.01f)
            {
                StopMovement();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning("Destination not reached in time, skipping.");
    }


    private IEnumerator UnloadItems(string itemType)
    {
        if (itemsCount > 0)
        {
            var hq = Base.GetBaseById(baseId);
            yield return new WaitForSeconds(hq.GetTransferTime());
            hq.TransferResources(itemType, itemsCount);
        }
    }

    private IEnumerator CancelReturnAndUnload()
    {
        MoveTo(baseTransform.position);

        yield return new WaitUntil(() => !pathFinder.pathPending && pathFinder.reachedDestination);

        StopMovement();
        yield return UnloadItems(currentItemId);

        cancelCoroutine = null;
        itemsCount = 0;
    }
}
