using UnityEngine;

public class Ending : Action
{
    public override void Act()
    {
        UIHandler.Instance.IsInteractable = true;
    }
    public override void CancelAct()
    {
        UIHandler.Instance.IsInteractable = false;
    }
}
