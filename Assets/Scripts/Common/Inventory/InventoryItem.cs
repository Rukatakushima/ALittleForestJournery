using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : BaseItem
{
    [SerializeField] private const float MAX_SPRITE_SIZE = 0.4f;
    private SceneItem linkedSceneItem;
    private Slot[] inventory;

    private void Start() => inventory = Inventory.Instance.slots;

    public void Initialize(int id, SceneItem sceneItem, Sprite sprite)
    {
        ID = id;
        linkedSceneItem = sceneItem;

        GetComponent<Image>().sprite = sprite;
        GetComponent<RectTransform>().localScale = sprite.bounds.size * MAX_SPRITE_SIZE;
    }

    public override void Spawn()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].isFull)
            {
                inventory[i].AddItem(transform);
                Show();
                break;
            }
        }
    }

    public void Drop()
    {
        Hide();
        transform.SetParent(null);

        linkedSceneItem.Spawn();
    }
}