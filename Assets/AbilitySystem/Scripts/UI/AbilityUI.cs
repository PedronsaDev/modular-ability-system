using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private List<AbilitySlotUI> _slotUIs;
    [SerializeField] private DraggableAbilityUI _draggableAbilityPrefab;
    [SerializeField] private Transform _draggablesParent;
    [SerializeField] private List<AbilityData> _allAbilities;

    private void OnValidate()
    {
        _allAbilities = Extensions.GetAllInstances<AbilityData>().ToList();
    }

    private void Start()
    {
        foreach (var ability in _allAbilities)
        {
            var draggableAbilityUI = Instantiate(_draggableAbilityPrefab, _draggablesParent);
            draggableAbilityUI.Setup(ability);
        }
        _draggablesParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
            ToggleInventory();
    }
    private void ToggleInventory()
    {
        _draggablesParent.gameObject.SetActive(!_draggablesParent.gameObject.activeSelf);
    }

    public void BindSlots(AbilitySlot[] slots)
    {
        for (int i = 0; i < _slotUIs.Count && i < slots.Length; i++)
            _slotUIs[i].Bind(slots[i], i+1);
    }
}
