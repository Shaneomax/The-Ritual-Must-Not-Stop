using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    //public Sprite uiIcon;
    public ItemType type; 
}

public enum ItemType 
{ 
    DemonBook,   
    Candle,     
    Chalk,
    Skull,
    Key,
    Lighter 
}