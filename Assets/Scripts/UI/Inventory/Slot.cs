using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory inventory;
    public int i;
    public GameObject player;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (transform.childCount <= 0)
        {
            inventory.isFull[i] = false;
        }
    }

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            int id = child.GetComponent<ItemInInventory>().id;
            ItemsManager.Instance.SpawnOnScene(id);
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
                    child.SetParent(inventory.slots[i].transform);
                    child.position = inventory.slots[i].transform.position;

                    inventory.isFull[i] = true;
                    i--;
                    inventory.isFull[i] = false;
                    i++;
                    break;
                }
            }
            i--;
        }
    }
}
