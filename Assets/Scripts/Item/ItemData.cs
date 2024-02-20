using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 2)]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public string description;
        public ItemType itemType;
        public Sprite sprite;
        public int weight;
    }
}