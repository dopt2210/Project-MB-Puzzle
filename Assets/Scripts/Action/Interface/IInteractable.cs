using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; set; }
    bool IsInRange { get; set; }
    public void Interact();
    public void DisableInteract();
    public void SetInteract(bool value);
}
