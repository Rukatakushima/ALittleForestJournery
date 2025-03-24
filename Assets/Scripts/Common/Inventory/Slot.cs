using UnityEngine;

public class Slot : MonoBehaviour
{
    public int index;
    public bool isFull;

    public void AddItem(Transform item)
    {
        item.SetParent(transform);
        item.SetAsFirstSibling();
        item.position = transform.position;
        isFull = true;
    }

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            InventoryItem childItem = child.GetComponent<InventoryItem>();
            if (childItem != null)
            {
                childItem.Drop();
                isFull = false;
            }
        }
    }

    public void PlaceInNextSlot(Inventory inventory)
    {
        if (isFull != true || inventory == null) return;

        Slot nextSlot = inventory.slots[index + 1];
        if (nextSlot.isFull != false) return;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<InventoryItem>() != null)
            {
                nextSlot.AddItem(child);
                isFull = false;
                break;
            }
        }
    }
}