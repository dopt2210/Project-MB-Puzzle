using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryObject", menuName = "Scriptable Objects/InventoryObject")]
public class InventoryObject : ScriptableObject
{
    public int maxSlots = 5;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public bool AddItem(ItemSO item, int quantity = 1)
    {
        if (item.stackable)
        {
            var slot = Container.Find(s => s.item == item);
            if (slot != null)
            {
                slot.AddQuantity(quantity);
                return true;
            }
        }

        if (Container.Count < maxSlots)
        {
            Container.Add(new InventorySlot(item, quantity));
            return true;
        }

        Debug.Log("Inventory full!");
        return false;
    }
    public void RemoveItem(ItemSO item, int quantity = 1)
    {
        var slot = Container.Find(s => s.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0) Container.Remove(slot);
        }
    }
}