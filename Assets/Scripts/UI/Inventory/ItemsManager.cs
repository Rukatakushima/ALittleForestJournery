using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager Instance { get; private set; }

    // public bool createListOfItemsInInspector = false;
    // #if UNITY_EDITOR
    //     void OnValidate()
    //     {
    //         if (createListOfItemsInInspector)
    //             PopulateItemPrefabs();
    //     }
    // #endif

    [SerializeField] private Transform player;
    private Inventory inventory;
    public List<GameObject> itemsOnScenePrefabs;

    public double maxSpriteSize = 0.68;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        inventory = player.gameObject.GetComponent<Inventory>();
    }

    private void Start() => PopulateItemPrefabs();

    private void PopulateItemPrefabs()
    {
        ItemOnScene[] pickups = FindObjectsOfType<ItemOnScene>();
        foreach (ItemOnScene pickup in pickups)
        {
            GameObject prefab = pickup.gameObject;
            if (prefab != null && !itemsOnScenePrefabs.Contains(prefab))
            {
                itemsOnScenePrefabs.Add(prefab);
                if (prefab.transform.parent != this)
                    prefab.transform.SetParent(transform);
            }
        }
    }

    public void SpawnOnScene(int itemId)
    {
        GameObject item = GetItemByIdOnScene(itemId);
        Vector2 spawnPos = new Vector2(player.position.x + item.transform.localScale.x, player.position.y);
        item.SetActive(true);
        item.transform.position = spawnPos;
        item.GetComponent<ItemOnScene>().spawnedItemInInventoryGameObject.SetActive(false);
        item.GetComponent<ItemOnScene>().spawnedItemInInventoryGameObject.transform.SetParent(null);
    }

    public GameObject GetItemByIdOnScene(int itemId)
    {
        foreach (GameObject itemPrefab in itemsOnScenePrefabs)
        {
            GameObject pickup = itemPrefab.GetComponent<ItemOnScene>().gameObject;
            if (pickup != null && pickup.GetComponent<ItemOnScene>().id == itemId)
            {
                return pickup;
            }
        }

        return null;
    }

    public void SpawnInInventory(GameObject itemInInventory, GameObject itemOnScene, int itemId)
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (!inventory.isFull[i])
            {
                inventory.isFull[i] = true;

                itemInInventory.SetActive(true);
                SetItemSpriteInInventory(itemInInventory, itemId);
                itemInInventory.transform.SetParent(inventory.slots[i].transform, false);
                itemInInventory.transform.SetAsFirstSibling();
                itemInInventory.transform.position = inventory.slots[i].transform.position;
                itemInInventory.GetComponent<ItemInInventory>().id = itemId;
                itemInInventory.name = itemOnScene.name;


                itemOnScene.SetActive(false);
                break;
            }
        }
    }

    public void SetItemSpriteInInventory(GameObject itemInInventory, int itemId)
    {
        Sprite itemSprite = GetItemByIdOnScene(itemId).GetComponent<SpriteRenderer>().sprite;
        itemInInventory.GetComponent<Image>().sprite = itemSprite;
        SetItemSizeInInventory(itemInInventory, itemSprite.bounds.size);
    }

    public void SetItemSizeInInventory(GameObject itemInInventory, Vector2 spriteSize)
    {
        if (spriteSize.x > maxSpriteSize || spriteSize.y > maxSpriteSize)
            spriteSize = spriteSize * (float)maxSpriteSize;

        itemInInventory.GetComponent<RectTransform>().localScale = spriteSize;
    }
}
