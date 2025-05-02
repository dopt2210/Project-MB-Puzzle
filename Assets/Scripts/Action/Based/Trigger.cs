using UnityEngine;

public class Trigger : MonoBehaviour
{
    public ActionTriggerCtrl triggerActionCtrl;

    protected virtual void Awake()
    {
        triggerActionCtrl = transform.GetComponentInParent<ActionTriggerCtrl>();
    }
    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
        {
            action.Act();
        }

    }

    protected virtual void OnTriggerStay(Collider collision)
    {
        if (triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
        {
            action.UpdateAct();
        }

    }

    protected virtual void OnTriggerExit(Collider collision)
    {
        if (triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
        {
            action.CancelAct();
        }
    }
}
