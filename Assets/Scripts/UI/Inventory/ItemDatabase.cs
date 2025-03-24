using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    private Dictionary<int, ItemData> itemsDictionary;

    public void Initialize() => itemsDictionary = items.ToDictionary(item => item.id, item => item);

    public ItemData GetItemById(int id) => itemsDictionary.TryGetValue(id, out var item) ? item : null;
}

[System.Serializable]
public class ItemData
{
    public int id;
    public GameObject scenePrefab;
    public Sprite inventorySprite;
    public float maxSpriteSize = 0.4f;
}