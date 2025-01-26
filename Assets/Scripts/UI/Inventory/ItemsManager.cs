using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class ItemsManager : MonoBehaviour
{
    public static ItemsManager Instance { get; private set; }
    public bool createListOfItemsInInspector = false;
    #region  SAVE_PREFABS
#if UNITY_EDITOR
    void OnValidate()
    {
        if (createListOfItemsInInspector)
        {
            PopulateItemPrefabs();
        }
    }
#endif
    private void PopulateItemPrefabs()
    {
        ItemOnScene[] pickups = FindObjectsOfType<ItemOnScene>();
        foreach (ItemOnScene pickup in pickups)
        {
            // GameObject prefab = prefabFromInstance(pickup.gameObject);
            GameObject prefab = pickup.gameObject;
            if (prefab != null && !itemsOnScenePrefabs.Contains(prefab))
            {
                itemsOnScenePrefabs.Add(prefab);
                if (prefab.transform.parent != this)
                {
                    prefab.transform.SetParent(this.transform);
                }
            }
        }
    }
    private GameObject prefabFromInstance(GameObject instance)
    {
        return PrefabUtility.GetCorrespondingObjectFromSource(instance);
    }
    #endregion

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
    }

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        inventory = player.gameObject.GetComponent<Inventory>();
        PopulateItemPrefabs();
    }

    public void SpawnOnScene(int itemId)
    {
        Vector2 playerPos = new Vector2(player.position.x + 1, player.position.y);
        GameObject item = GetItemByIdOnScene(itemId);
        item.SetActive(true);
        item.transform.position = playerPos;
        item.GetComponent<ItemOnScene>().spawnedItemInInventoryGameObject.SetActive(false);
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

                // GameObject spawnedItemInInventory = Instantiate(itemInInventory, inventory.slots[i].transform);
                // GameObject spawnedItemInInventory = itemInInventory.GetFromPool();

                SetItemSpriteInInventory(itemInInventory, itemId);
                itemInInventory.transform.SetParent(inventory.slots[i].transform, false);
                itemInInventory.GetComponent<ItemInInventory>().id = itemId;
                itemInInventory.name = itemOnScene.name;

                // itemsInInventory.Add(spawnedItemInInventory);

                itemOnScene.SetActive(false);
                // Destroy(itemOnScene);
                // itemOnScene.GetComponent<PoolBase>().ReturnToPool(itemOnScene);
                break;
            }
        }
    }

    public void SetItemSpriteInInventory(GameObject itemInInventory, int itemId)
    {
        // GameObject itemFromList = GetItemById(itemId);
        Sprite itemSprite = GetItemByIdOnScene(itemId).GetComponent<SpriteRenderer>().sprite;
        itemInInventory.GetComponent<Image>().sprite = itemSprite;
        SetItemSizeInInventory(itemInInventory, itemSprite.bounds.size);
    }

    public void SetItemSizeInInventory(GameObject itemInInventory, Vector2 spriteSize)
    {
        if (spriteSize.x > maxSpriteSize || spriteSize.y > maxSpriteSize)
        {
            spriteSize = spriteSize * (float)maxSpriteSize;
        }
        // else if (spriteSize.x < minSpriteSize || spriteSize.y < minSpriteSize)
        // {
        //     spriteSize = spriteSize / (float)maxSpriteSize;
        // }
        itemInInventory.GetComponent<RectTransform>().localScale = spriteSize;
    }
}
