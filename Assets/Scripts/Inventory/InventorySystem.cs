using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    public int MaxSlots = 32;
    public List<InventoryData> InventoryDatas = new List<InventoryData>();

    public event System.Action<int> OnItemAdd;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        for (int i = 0; i < MaxSlots; i++)
            InventoryDatas.Add(null);
    }

    public bool AddItem(ItemSO item, int quantity = 1)
    {
        if (item == null) return false;

        if (item.stackable)
        {
            for (int i = 0; i < InventoryDatas.Count; i++)
            {
                if (InventoryDatas[i]?.item == item)
                {
                    InventoryDatas[i].quantity++;
                    OnItemAdd?.Invoke(i);
                    return true;
                }
            }
        }
        for (int i = 0; i < InventoryDatas.Count; i++)
        {
            if (!IsValidData(InventoryDatas[i]))
            {
                InventoryDatas[i] = new InventoryData(item, quantity);
                OnItemAdd?.Invoke(i);
                return true;
            }
        }


        Debug.Log("Inventory full!");
        return false;
    }
    private bool IsValidData(InventoryData data)
    {
        return data != null && data.item != null && data.quantity > 0;
    }

    public InventoryData GetItemAt(int index)
    {
        return InventoryDatas[index];
    }
    public void RemoveItemAt(int index)
    {
        InventoryDatas[index] = null;

    }
    public void NotifyInventoryChanged()
    {
        //OnInventoryChanged?.Invoke();
    }

}
