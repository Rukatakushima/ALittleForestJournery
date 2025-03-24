using UnityEngine;
public class SceneItem : BaseItem
{
    [SerializeField] private InventoryItem inventoryItemPrefab;
    private InventoryItem linkedInventoryItem;
    // private Slot[] inventory;
    private Mover player;

    private void Awake()
    {
        linkedInventoryItem = Instantiate(inventoryItemPrefab.gameObject).GetComponent<InventoryItem>();
        linkedInventoryItem.Initialize(ID, this, GetComponent<SpriteRenderer>().sprite);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>();
    }

    // private void Start() => inventory = Inventory.Instance.slots;

    public void PickUp()
    {
        Hide();
        linkedInventoryItem.Spawn();
    }

    // public void SpawnInInventory()
    // {
    //     for (int i = 0; i < inventory.Length; i++)
    //     {
    //         if (!inventory[i].isFull)
    //         {
    //             inventory[i].AddItem(inventoryItem.transform);
    //             inventoryItem.Show();
    //             break;
    //         }
    //     }
    // }

    public override void Spawn()
    {
        Vector2 spawnPos;
        Transform playerTransform = player.gameObject.transform;

        if (player.isFacingLeft)
            spawnPos = new Vector2(playerTransform.position.x - transform.localScale.x, playerTransform.position.y);
        else
            spawnPos = new Vector2(playerTransform.position.x + transform.localScale.x, playerTransform.position.y);

        Show();
        transform.position = spawnPos;
    }
}