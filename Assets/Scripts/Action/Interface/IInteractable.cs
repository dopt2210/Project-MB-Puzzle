using UnityEngine;

public interface IInteractable
{
    GameObject PlayerGameObject { get; set; }
    bool IsInteractable { get; set; }
    public void Interact();
    public void DisableInteract();
    public void SetRangeInteract(bool value);
}
