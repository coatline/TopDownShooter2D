using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public Slot selectedSlot;
    public List<Slot> slots;
    int selSlotIndex;

    void Start()
    {
        selectedSlot = slots[selSlotIndex];
        selectedSlot.GetComponent<Image>().color = Color.white;
    }

    private void Update()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0)
        {
            ChangeSelected(-1);
        }
        else if (scrollInput < 0)
        {
            ChangeSelected(1);
        }
    }

    public Slot OpenSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item)
            {
                continue;
            }
            else
            {
                return slots[i];
            }
        }

        return null;
    }

    void ChangeSelected(int amount)
    {
        selectedSlot.GetComponent<Image>().color = Color.black;

        if ((selSlotIndex) + amount > slots.Count - 1)
        {
            selSlotIndex = 0;
        }
        else if ((selSlotIndex) + amount < 0)
        {
            selSlotIndex = slots.Count - 1;
        }
        else
        {
            selSlotIndex += amount;
        }

        selectedSlot = slots[selSlotIndex];

        selectedSlot.GetComponent<Image>().color = Color.white;
    }
}
