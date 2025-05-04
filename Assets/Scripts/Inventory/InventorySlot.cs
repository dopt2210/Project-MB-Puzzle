using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        GameObject dropItem = eventData.pointerDrag;
        InventoryItem dragItem = dropItem.GetComponent<InventoryItem>();
        if (dragItem == null) return;

        Transform fromSlot = dragItem.tParrent;
        Transform toSlot = transform;

        if (fromSlot == toSlot) return;

        int fromIndex = fromSlot.GetSiblingIndex();
        int toIndex = toSlot.GetSiblingIndex();

        SwapData(fromIndex, toIndex);

        Transform toItem = toSlot.childCount > 0 ? toSlot.GetChild(0) : null;

        dragItem.transform.SetParent(toSlot);
        dragItem.tParrent = toSlot;

        if (toItem != null)
        {
            toItem.SetParent(fromSlot);
            toItem.GetComponent<InventoryItem>().tParrent = fromSlot;
        }
    }
    private void SwapData(int fromIndex, int toIndex)
    {
        InventorySystem inv = InventorySystem.Instance;
        InventoryData temp = inv.InventoryDatas[fromIndex];
        inv.InventoryDatas[fromIndex] = inv.InventoryDatas[toIndex];
        inv.InventoryDatas[toIndex] = temp;
    }
}
