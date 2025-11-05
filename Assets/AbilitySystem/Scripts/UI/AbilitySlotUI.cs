using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    private AbilitySlot _slot;

    public void Bind(AbilitySlot slot)
    {
        _slot = slot;
        _slot.OnInitialize += UpdateUI;
        _slot.OnCooldownStart += OnCooldownStart;
        _slot.OnCooldownComplete += OnCooldownComplete;
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
    }
}
