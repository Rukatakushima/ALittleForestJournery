using System.Collections.Generic;
using ObjectsPool;
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
            GameObject prefab = prefabFromInstance(pickup.gameObject);
            if (prefab != null && !itemsOnScenePrefabs.Contains(prefab))
            {
                itemsOnScenePrefabs.Add(prefab);
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

    public List<GameObject> itemsInInventory;
    private GameObjectPool itemInInventoryObjectPool;
    [SerializeField] private const int itemInInventoryPreloadCount = 1;

    public double maxSpriteSize = 0.68;

    /*
    // public double minSpriteSize = 0.4;

    // //установка в Awake
    // private GameObject itemsObjectPrefab;
    // public void GetAction(GameObject gameObject) => gameObject.SetActive(true);
    // public void ReturnAction(GameObject gameObject) => gameObject.SetActive(false);
    // public GameObject Preload() => Instantiate(itemsObjectPrefab);
    // private PoolBase<GameObject> itemPool;
    // [SerializeField] private const int itemPreloadCount = 5;

    // Инициализация
    // private GameObjectPool itemsObjectsPool;
*/
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        /*
                // itemPool = new PoolBase<GameObject>(Preload, GetAction, ReturnAction, itemPreloadCount);
                // itemsObjectsPool = new GameObjectPool(itemsObjectPrefab, itemPreloadCount);

                // GameObject itemObject = itemsObjectsPool.GetFromPool();// Вызов
                // itemsObjectsPool.ReturnAllToPool();// Отзыв

                // Sprite itemSprite = itemObject.GetComponent<SpriteRenderer>().sprite;
                */
        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            itemInInventoryObjectPool = new GameObjectPool(itemsInInventory[i], itemInInventoryPreloadCount);
        }
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
        ItemOnScene item = GetItemByIdOnScene(itemId);
        Vector2 playerPos = new Vector2(player.position.x + 1, player.position.y);
        // item.GetFromPool();
        Instantiate(item, playerPos, Quaternion.identity);

        // itemsInInventory.Remove(item);
    }

    public ItemOnScene GetItemByIdOnScene(int itemId)
    {
        foreach (GameObject itemPrefab in itemsOnScenePrefabs)
        {
            ItemOnScene pickup = itemPrefab.GetComponent<ItemOnScene>();
            if (pickup != null && pickup.id == itemId)
            {
                return pickup;
            }
        }

        return null;
    }

    public void SpawnInInventory(GameObjectPool itemInInventoryObjectPool, GameObject itemOnScene, int itemId)
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (!inventory.isFull[i])
            {
                inventory.isFull[i] = true;

                // GameObject spawnedItemInInventory = Instantiate(itemInInventoryObjectPool, inventory.slots[i].transform);
                GameObject spawnedItemInInventory = itemInInventoryObjectPool.GetFromPool();
                SetItemSpriteInInventory(spawnedItemInInventory, itemId);
                spawnedItemInInventory.transform.SetParent(inventory.slots[i].transform, false);

                spawnedItemInInventory.GetComponent<ItemInInventory>().id = itemId;
                spawnedItemInInventory.name = itemOnScene.name;
                // itemsInInventory.Add(spawnedItemInInventory);
                Destroy(itemOnScene);
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
