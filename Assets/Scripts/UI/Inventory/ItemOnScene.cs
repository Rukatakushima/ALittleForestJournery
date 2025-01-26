using UnityEngine;
using ObjectsPool;

public class ItemOnScene : MonoBehaviour
{
    [SerializeField] public GameObject itemInInventoryPrefab;
    [SerializeField] public int id;

    public GameObjectPool itemsInInventoryObjectPool;
    [SerializeField] private const int itemInInventoryPreloadCount = 1;


    // private GameObjectPool itemsOnSceneObjectPool;
    // [SerializeField] private const int itemOnScenePreloadCount = 0;

    public GameObject spawnedItemInInventoryGameObject;


    private void Awake()
    {
        itemsInInventoryObjectPool = new GameObjectPool(itemInInventoryPrefab, itemInInventoryPreloadCount);

        // itemsOnSceneObjectPool = new GameObjectPool(this.gameObject, itemOnScenePreloadCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // GameObject itemObject = itemsInInventoryObjectPool.GetFromPool();// Вызов

            // if (spawnedItemInInventoryGameObject == null)
            // {
                spawnedItemInInventoryGameObject = itemsInInventoryObjectPool.GetFromPool();
            // }

            ItemsManager.Instance.SpawnInInventory(spawnedItemInInventoryGameObject, this.gameObject, id);
        }
    }
}
