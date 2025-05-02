using UnityEngine;

public class Action : MonoBehaviour
{
    public ActionTriggerCtrl triggerActionCtrl;
    public virtual void Awake()
    {
        triggerActionCtrl = GetComponentInParent<ActionTriggerCtrl>();
    }
    public virtual void Act() { }
    public virtual void UpdateAct() { }
    public virtual void CancelAct() { }
}
