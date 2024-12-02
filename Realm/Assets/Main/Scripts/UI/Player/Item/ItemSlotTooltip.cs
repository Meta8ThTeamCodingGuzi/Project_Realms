using DarkPixelRPGUI.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipWindow tooltipWindow;
    [SerializeField] private Slot slot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot.IsEmpty()) return;

        tooltipWindow.ShowTooltip(
            slot.Item.ItemID.ToString(),
            slot.Item.GetTooltip(),
            transform.position.x
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipWindow.HideTooltip();
    }
}