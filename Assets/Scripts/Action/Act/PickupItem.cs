public class PickupItem : Action
{
    public ItemSO itemData;

    public override void Act()
    {
        bool success = InventorySystem.Instance.AddItem(itemData);
        if (success)
        {
            Destroy(triggerActionCtrl.transform.parent.gameObject);
        }
    }
}
