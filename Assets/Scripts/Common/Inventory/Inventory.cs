using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public Slot[] slots;

    private void Awake()
    {
        Instance = this;
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        slots = GetComponentsInChildren<Slot>(true)
            .Select((slot, i) =>
            {
                slot.SetIndex(i);
                return slot;
            })
            .ToArray();
        // int i = 0;
        // foreach (Transform child in transform)
        // {
        //     Slot childItem = child.GetComponent<Slot>();
        //     if (childItem != null && !slots.Contains(childItem))
        //     {
        //         slots[i] = childItem;
        //         childItem.SetIndex(i);
        //         i++;
        //     }
        // }
    }

    public bool TryFindFirstEmptySlot(out Slot slot)
    {
        slot = slots.FirstOrDefault(s => !s.IsFull);
        return slot != null;
    }
}