public interface IInteractable
{
    bool IsInteractable { get; set; }
    bool IsInRange { get; set; }
    public void DoInteract();
    public void DisableInteract();
    public event System.Action<bool> OnRangeChanged; // Optional: Event to notify when interaction occurs
}
