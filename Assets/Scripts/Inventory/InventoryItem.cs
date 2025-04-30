using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : BaseItem
{
    private SceneItem linkedSceneItem;

    public void Initialize(string id, SceneItem sceneItem, Sprite sprite)
    {
        ID = id;
        linkedSceneItem = sceneItem;

        Image image = GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
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