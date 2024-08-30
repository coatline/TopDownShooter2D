using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] Player player = null;
    public ItemDisplay selectedSlot = null;
    public List<ItemDisplay> slots = null;
    int selSlotIndex = 0;

    void Start()
    {
        selectedSlot = slots[selSlotIndex];
        player.selectedSlot = selectedSlot;
        selectedSlot.Select();
    }

    private void Update()
    {
        if (!player) { return; }

        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0)
        {
            ChangeSelected(1);
        }
        else if (scrollInput < 0)
        {
            ChangeSelected(-1);
        }
    }

    public ItemDisplay OpenSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentItemScript)
            {
                continue;
            }
            else
            {
                return slots[i];
            }
        }

        return selectedSlot;
    }

    public ItemDisplay ContatinsItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].currentItemScript) { continue; }

            if (slots[i].currentItemScript.itemName == item.itemName)
            {
                return slots[i];
            }
        }

        return null;
    }

    void ChangeSelected(int amount)
    {
        selectedSlot.DeSelect();

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

        selectedSlot.Select();

        player.selectedSlot = selectedSlot;
    }

}
