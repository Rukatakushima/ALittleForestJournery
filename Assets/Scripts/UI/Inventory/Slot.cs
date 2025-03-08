using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory inventory;
    public int i;
    // public GameObject player;

    private void Awake() => inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    // player = GameObject.FindGameObjectWithTag("Player");

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            ItemInInventory childItem = child.GetComponent<ItemInInventory>();
            if (childItem != null)
            {
                ItemsManager.Instance.SpawnOnScene(childItem.id);
                inventory.isFull[i] = false;
            }
        }
    }

    public void PlaceInNextSlot()
    {
        if (inventory.isFull[i] == true)
        {
            i++;
            if (inventory.isFull[i] == false)
            {
                foreach (Transform child in transform)
                {
                    ItemInInventory childItem = child.GetComponent<ItemInInventory>();
                    if (childItem != null)
                    {
                        child.SetParent(inventory.slots[i].transform);
                        child.SetAsFirstSibling();
                        child.position = inventory.slots[i].transform.position;

                        inventory.isFull[i] = true;
                        i--;
                        inventory.isFull[i] = false;
                        i++;
                        break;
                    }
                }
            }
            i--;
        }
    }
}
