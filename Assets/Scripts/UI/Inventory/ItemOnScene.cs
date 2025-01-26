using UnityEngine;
using ObjectsPool;

public class ItemOnScene : MonoBehaviour
{
    [SerializeField] public GameObject itemInInventoryPrefab;
    [SerializeField] public int id;

    public GameObjectPool itemsInInventoryObjectPool;
    [SerializeField] private const int itemInInventoryPreloadCount = 1;

    public GameObject spawnedItemInInventoryGameObject;


    private void Awake()
    {
        itemsInInventoryObjectPool = new GameObjectPool(itemInInventoryPrefab, itemInInventoryPreloadCount);
    }

    private void Start()
    {
        spawnedItemInInventoryGameObject = itemsInInventoryObjectPool.GetFromPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            ItemsManager.Instance.SpawnInInventory(spawnedItemInInventoryGameObject, this.gameObject, id);
        }
    }
}
