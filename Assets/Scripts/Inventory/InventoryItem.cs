using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : BaseItem
{
    [SerializeField] private const float MAX_SPRITE_SIZE = 0.4f;
    private SceneItem linkedSceneItem;

    public void Initialize(int id, SceneItem sceneItem, Sprite sprite)
    {
        ID = id;
        linkedSceneItem = sceneItem;

        GetComponent<Image>().sprite = sprite;
        GetComponent<RectTransform>().localScale = MAX_SPRITE_SIZE * sprite.bounds.size;
    }

    public override void Spawn()
    {
        if (Inventory.Instance == null) return;

        if (Inventory.Instance.TryFindFirstEmptySlot(out Slot slot))
        {
            slot.TryAddItem(transform);
            Show();
        }
    }

    public void Drop()
    {
        Hide();
        transform.SetParent(null);

        linkedSceneItem?.Spawn();
    }
}