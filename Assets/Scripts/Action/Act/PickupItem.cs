using System.Diagnostics;

public class PickupItem : Action, IInteractable
{
    private bool _isInRange;
    public ItemSO itemData;
    public bool IsInteractable { get => InputManager.Instance.Action.Interact; set { } }
    public bool IsInRange
    {
        get => _isInRange; set
        {
            if (_isInRange != value)
            {
                _isInRange = value;
                OnRangeChanged?.Invoke(_isInRange);
                UIHandler.Instance.RegisterTarget(this);
            }
        }
    }

    public event System.Action<bool> OnRangeChanged;

    public override void Act()
    {
        IsInRange = true;
    }
    public override void CancelAct()
    {
        IsInRange = false;
    }

    public void DisableInteract() { }

    public void DoInteract()
    {
        IsInRange = false;
        var success = InventorySystem.Instance.AddItem(itemData);
        if (success)
        {
            Destroy(triggerActionCtrl.transform.parent.gameObject);
            if (CheckCurrentCell())
            {
                MazeGenerator.KeyPoint.RemoveKeyPoint(itemData.algorithmType, GameManager.Instance.CurrentCell);
            }
            // else
            // {
            //     NotifyManager.Instance.Notify($"You have picked up {itemData.itemName}");
            // }
        }
        NotifyManager.Instance.StartNotify(itemData);
    }
    private bool CheckCurrentCell()
    {
        foreach(var cell in MazeGenerator.KeyPoint.GetKeyPoints(itemData.algorithmType))
        {
            if (cell == GameManager.Instance.CurrentCell)
            {
                return true;
            }
        }
        return false;
    }
    private void Update()
    {
        if (IsInteractable && IsInRange)
        {
            DoInteract();
        }
    }
}
