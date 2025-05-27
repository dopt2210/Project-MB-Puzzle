using UnityEngine;

public class Action : MonoBehaviour
{
    public TriggerActionCtrl triggerActionCtrl { get; private set; }
    public virtual void Awake()
    {
        triggerActionCtrl = GetComponentInParent<TriggerActionCtrl>();
    }
    public virtual void Act() { }
    public virtual void UpdateAct() { }
    public virtual void CancelAct() { }
}
