using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YourNamespace;

/// <summary>
/// UI representation of an ability slot; binds to slot events and visualizes cooldown via fill & tween.
/// </summary>
public class AbilitySlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _numberText;
    private AbilitySlot _slot;

    /// <summary>Bind this UI to a slot and initialize numeric label.</summary>
    public void Bind(AbilitySlot slot, int index)
    {
        _slot = slot;
        _slot.OnInitialize += UpdateUI;
        _slot.OnCooldownStart += OnCooldownStart;
        _slot.OnCooldownComplete += OnCooldownComplete;

        if (index == 10)
            index = 0;

        _numberText.SetText(index.ToString());
    }

    private void OnDestroy()
    {
        if (_slot == null)
            return;

        _slot.OnInitialize -= UpdateUI;
        _slot.OnCooldownStart -= OnCooldownStart;
        _slot.OnCooldownComplete -= OnCooldownComplete;
    }

    /// <summary>Cooldown complete visuals reset.</summary>
    private void OnCooldownComplete()
    {
        _iconImage.fillAmount = 1f;
        _iconImage.color = Color.white;
    }

    /// <summary>Cooldown start visuals & tween invocation.</summary>
    private void OnCooldownStart()
    {
        _iconImage.fillAmount = 0f;
        _iconImage.color = Color.gray;

        Tween.UIFillAmount(_iconImage, 1f, _slot.Ability.CooldownTime);
    }

    private void UpdateUI(AbilitySlot newSlot)
    {
        _iconImage.sprite = newSlot.Ability.Icon;
        _iconImage.color = Color.white;
    }
    /// <summary>Handle drop of a draggable ability icon to set the slot's ability.</summary>
    public void OnDrop(PointerEventData eventData)
    {
        var draggableAbility = DraggableAbilityUI.CurrentlyDraggedAbility;
        if (draggableAbility && draggableAbility.AbilityData)
            _slot.SetAbility(draggableAbility.AbilityData);
        Destroy(draggableAbility.gameObject);
    }
}
