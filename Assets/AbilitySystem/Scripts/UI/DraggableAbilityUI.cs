using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableAbilityUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Image _iconImage;
    public AbilityData AbilityData;

    public static DraggableAbilityUI CurrentlyDraggedAbility;

    public void Setup(AbilityData abilityData)
    {
        AbilityData = abilityData;
        if (AbilityData && _iconImage)
            _iconImage.sprite = AbilityData.Icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CurrentlyDraggedAbility = this;
        _iconImage.color = new Color(255,255,255,0.6f);
        _iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _iconImage.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 globalMousePos))
        {
            _iconImage.rectTransform.position = globalMousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _iconImage.raycastTarget = true;
        CurrentlyDraggedAbility = null;
        _iconImage.color = Color.white;
        _iconImage.rectTransform.localPosition = Vector3.zero;
    }
}
