using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    public GameObject inventory;
    private bool inventoryOn;

    private void Start() => inventoryOn = false;

    public void ToggleChest()
    {
        inventoryOn = !inventoryOn;
        inventory.SetActive(inventoryOn);
    }
}
