using UnityEngine;
using UnityEngine.Events;
public class SceneItem : BaseItem
{
    [SerializeField] private InventoryItem inventoryItemPrefab;
    private InventoryItem linkedInventoryItem;
    private Mover player;

    [SerializeField] public UnityEvent OnItemPickedUp;
    [SerializeField] public UnityEvent OnItemDropped;

    private void Awake()
    {
        linkedInventoryItem = Instantiate(inventoryItemPrefab);
        linkedInventoryItem.Initialize(ID, this, GetComponent<SpriteRenderer>().sprite);
        linkedInventoryItem.Hide();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Mover>();
    }

    public void PickUp()
    {
        Hide();
        OnItemPickedUp?.Invoke();
        linkedInventoryItem.Spawn();
    }

    public override void Spawn()
    {
        transform.position = (Vector2)player.transform.position + (player.isFacingLeft ? Vector2.left : Vector2.right);
        Show();
        OnItemDropped?.Invoke();
    }
}