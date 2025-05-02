using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public int MaxSlots = 32;
    public List<InventorySlot> Slots = new List<InventorySlot>();

    public event System.Action OnInventoryChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public bool AddItem(ItemSO item, int quantity = 1)
    {
        if (item.stackable)
        {
            var slot = Slots.Find(s => s.item == item);
            if (slot != null)
            {
                slot.AddQuantity(quantity);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        if (Slots.Count < MaxSlots)
        {
            Slots.Add(new InventorySlot(item, quantity));
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("Inventory full!");
        return false;
    }

    public void RemoveItem(ItemSO item, int quantity = 1)
    {
        var slot = Slots.Find(s => s.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0) Slots.Remove(slot);
            OnInventoryChanged?.Invoke();
        }
    }

    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

}
