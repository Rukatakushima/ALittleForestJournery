using UnityEngine;

public class Slot : MonoBehaviour
{
    private int Index;
    public bool IsFull { get; private set; }

    public void SetIndex(int index) => Index = index;

    public bool TryAddItem(Transform item)
    {
        if (IsFull || item == null) return false;

        item.SetParent(transform);
        item.SetAsFirstSibling();
        item.localPosition = Vector2.zero;
        IsFull = true;
        return true;
    }

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out InventoryItem item))
            {
                item.Drop();
                IsFull = false;
                break;
            }
        }
    }

    public void PlaceInNextSlot()
    {
        if (!IsFull || Inventory.Instance == null) return;

        int nextIndex = Index + 1;
        if (nextIndex >= Inventory.Instance.slots.Length) return;

        Slot nextSlot = Inventory.Instance.slots[nextIndex];
        if (nextSlot == null || nextSlot.IsFull) return;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<InventoryItem>() != null)
            {
                if (nextSlot.TryAddItem(child))
                {
                    IsFull = false;
                    break;
                }
            }
        }
    }
}