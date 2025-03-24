using UnityEngine;
public class SceneItem : MonoBehaviour
{
    [SerializeField] public int id;
    [SerializeField] private InventoryItem inventoryItemPrefab;
    private InventoryItem inventoryItem;
    private Slot[] inventory;
    private Mover player;

    private void Awake()
    {
        inventoryItem = Instantiate(inventoryItemPrefab.gameObject).GetComponent<InventoryItem>();
        inventoryItem.SetParameters(id, this, GetComponent<SpriteRenderer>().sprite);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>();
    }

    private void Start() => inventory = Inventory.Instance.slots;

    public void PickUp()
    {
        Hide();
        SpawnInInventory();
    }

    public void Hide() => gameObject.SetActive(false);

    public void Spawn()
    {
        Vector2 spawnPos;
        Transform playerTransform = player.gameObject.transform;

        if (player.isFacingLeft)
            spawnPos = new Vector2(playerTransform.position.x - transform.localScale.x, playerTransform.position.y);
        else
            spawnPos = new Vector2(playerTransform.position.x + transform.localScale.x, playerTransform.position.y);

        gameObject.SetActive(true);
        transform.position = spawnPos;
    }

    public void SpawnInInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].isFull)
            {
                inventory[i].AddItem(inventoryItem.transform);
                inventoryItem.Show();
                break;
            }
        }
    }
}