using UnityEngine;
using UnityEngine.Events;

public class SceneItem : BaseItem
{
    [SerializeField] private InventoryItem inventoryItemPrefab;
    private InventoryItem _linkedInventoryItem;
    private PlayerMovement _player;
    // private Mover _player;

    public UnityEvent onItemPickedUp;
    public UnityEvent onItemDropped;

    private void Awake()
    {
        _linkedInventoryItem = Instantiate(inventoryItemPrefab);
        _linkedInventoryItem.Initialize(ID, this, GetComponent<SpriteRenderer>().sprite);
        _linkedInventoryItem.Hide();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void PickUp()
    {
        Hide();
        onItemPickedUp?.Invoke();
        _linkedInventoryItem.Spawn();
    }

    public override void Spawn()
    {
        transform.position = (Vector2)_player.transform.position + (_player.IsFacingLeft ? Vector2.left : Vector2.right);
        Show();
        onItemDropped?.Invoke();
    }
}