using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] Player player = null;
    public Slot selectedSlot = null;
    public List<Slot> slots = null;
    int selSlotIndex = 0;

    void Start()
    {
        selectedSlot = slots[selSlotIndex];
        player.selectedSlot = selectedSlot;
        selectedSlot.Select();
    }

    public void SlotDisabledGroundItemFollow(Transform tran)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemHolder)
            {
                slots[i].itemHolder.transform.position = tran.position;
                slots[i].itemHolder.transform.rotation = tran.rotation;
            }
        }
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
