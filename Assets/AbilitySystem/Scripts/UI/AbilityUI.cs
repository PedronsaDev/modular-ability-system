using System.Collections.Generic;
using UnityEngine;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private List<AbilitySlotUI> _slotUIs;

    public void BindSlots(AbilitySlot[] slots)
    {
        for (int i = 0; i < _slotUIs.Count && i < slots.Length; i++)
            _slotUIs[i].Bind(slots[i]);
    }
}
