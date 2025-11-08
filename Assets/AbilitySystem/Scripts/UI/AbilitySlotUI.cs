using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _numberText;
    private AbilitySlot _slot;

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

    private void OnCooldownComplete()
    {
        _iconImage.fillAmount = 1f;
        _iconImage.color = Color.white;
    }

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
}
