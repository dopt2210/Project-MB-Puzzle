public class Ending : Action, IInteractable
{
    private bool _isInRange;
    public bool IsInteractable { get => InputManager.Instance.Action.Detail; set { } }
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
        NotifyManager.Instance.StartNotifyChoice("Do you want to continue?");
    }

    void Update()
    {
        if (IsInteractable && IsInRange)
        {
            DoInteract();
        }
    }
}
