public class PickupItem : Action
{
    public ItemSO itemData;

    public override void Act()
    {
        bool success = Inventory.Instance.AddItem(itemData);
        if (success)
        {
            Destroy(transform.root.gameObject);
        }
    }
}
