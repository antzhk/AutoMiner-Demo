using UnityEngine;

[CreateAssetMenu(fileName = "NewItemConfig", menuName = "Configs/Item Config")]
public class ItemConfig : ScriptableObject
{
    public string itemId;
    
    public float harvestSpeed;
}
