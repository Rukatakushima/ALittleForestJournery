using UnityEngine;
using ObjectsPool;

public class ItemOnScene : MonoBehaviour
{
    [SerializeField] public GameObject itemInInventoryPrefab;
    [SerializeField] public int id;

    private GameObjectPool itemsInInventoryObjectPool;
    [SerializeField] private const int itemPreloadCount = 1;


    private GameObjectPool itemsOnSceneObjectPool;
    [SerializeField] private const int itemOnScenePreloadCount = 0;

    private void Awake()
    {
       itemsInInventoryObjectPool = new GameObjectPool(itemInInventoryPrefab, itemPreloadCount);

        itemsOnSceneObjectPool = new GameObjectPool(this.gameObject, itemOnScenePreloadCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // GameObject itemObject = itemsInInventoryObjectPool.GetFromPool();// Вызов
            ItemsManager.Instance.SpawnInInventory(itemsInInventoryObjectPool, this.gameObject, id);
        }
    }
}
