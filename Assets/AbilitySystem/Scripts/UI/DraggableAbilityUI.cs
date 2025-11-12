using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Draggable representation of an ability; allows assigning abilities to slots via UI drag & drop.
/// </summary>
public class DraggableAbilityUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Image _iconImage;
    public AbilityData AbilityData;

    public static DraggableAbilityUI CurrentlyDraggedAbility;

    /// <summary>Initialize with AbilityData icon.</summary>
    public void Setup(AbilityData abilityData)
    {
        AbilityData = abilityData;
        if (AbilityData && _iconImage)
            _iconImage.sprite = AbilityData.Icon;
    }

    /// <summary>Begin drag: mark current instance and adjust visuals.</summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        CurrentlyDraggedAbility = this;
        _iconImage.color = new Color(255,255,255,0.6f);
        _iconImage.raycastTarget = false;
    }

    /// <summary>Update icon position following cursor.</summary>
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

    /// <summary>End drag: restore visuals and clear static reference.</summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        _iconImage.raycastTarget = true;
        CurrentlyDraggedAbility = null;
        _iconImage.color = Color.white;
        _iconImage.rectTransform.localPosition = Vector3.zero;
    }
}
