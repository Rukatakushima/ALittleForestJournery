using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public Slot[] slots;

    private void Awake()
    {
        Instance = this;
        SetSlots();
    }

    private void SetSlots()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            Slot childItem = child.GetComponent<Slot>();
            if (childItem != null && !slots.Contains(childItem))
            {
                slots[i] = childItem;
                childItem.index = i;
                i++;
            }
        }
    }
}