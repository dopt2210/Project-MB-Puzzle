using UnityEngine;
[RequireComponent (typeof(Collider))]
public class Trigger : MonoBehaviour
{
    public TriggerActionCtrl triggerActionCtrl { get; private set; }
    public Collider col {  get; private set; }
    public bool hasTrigger { get; private set; } = false;
    public int insideCount { get; private set; } = 0;
    protected virtual void Awake()
    {
        triggerActionCtrl = GetComponentInParent<TriggerActionCtrl>();
        col = GetComponent<Collider>();
        if (col == null) return;
        col.isTrigger = true;
    }
    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (!hasTrigger && triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
        {
            insideCount++;
            if (insideCount != 1) return;
            action.Act();
            hasTrigger = true;
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
        if (hasTrigger && triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
        {
            insideCount--;
            if (insideCount != 0) return;
            action.CancelAct();
            hasTrigger = false;
        }
    }
}
