using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public Image iconImage;
    public int slotIndex;

    private static GameObject dragIcon;
    private static RectTransform dragRect;
    private static Image dragImage;

    public Image icon;
    public TMP_Text qtyText;

    private ItemSO currentItem;

    public void Setup(ItemSO item, int quantity)
    {
        currentItem = item;
        icon.sprite = item.icon;
        qtyText.text = item.stackable ? quantity.ToString() : "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Inventory.Instance.Slots[slotIndex].item == null)
            return;
        

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root, false); // Canvas level
        dragRect = dragIcon.AddComponent<RectTransform>();
        dragImage = dragIcon.AddComponent<Image>();

        dragImage.sprite = iconImage.sprite;
        dragImage.raycastTarget = false;
        dragImage.SetNativeSize();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragRect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var fromSlotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (fromSlotUI != null)
        {
            SwapItems(fromSlotUI.slotIndex, this.slotIndex);
        }
    }
    private void SwapItems(int fromIndex, int toIndex)
    {
        var inv = Inventory.Instance;
        var temp = inv.Slots[fromIndex];
        inv.Slots[fromIndex] = inv.Slots[toIndex];
        inv.Slots[toIndex] = temp;

        inv.NotifyInventoryChanged();
    }
}
