using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    [System.Serializable]
    public class Item
    {
        public int id;
        [SerializeField] private ItemType _itemType;
        public int requireAdsNumber;
        public ItemType itemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }
    }

    public Item[] items;
}

public enum ItemType
{
    Money,
    Level,
    Free,
    WatchAds
}