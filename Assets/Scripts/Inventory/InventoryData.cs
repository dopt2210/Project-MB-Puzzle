[System.Serializable]
public class InventoryData
{
    public ItemSO item;
    public int quantity;

    public InventoryData(ItemSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void AddQuantity(int amount) => quantity += amount;
}
