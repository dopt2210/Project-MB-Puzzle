using UnityEngine;

public class TriggerPlayer : Trigger
{
    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
            {
                action.Act();
            }
        }
    }
    protected override void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
            {
                action.UpdateAct();
            }
        }
    }

    protected override void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerActionCtrl.triggerAndAction.TryGetValue(this, out Action action))
            {
                action.CancelAct();
            }
        }
    }
    public void SetRadiusTrigger(Vector2 size, Vector2 offset)
    {
        if (col == null)
        {
            Debug.Log("Not found coll");
            return;
        }
    }
}
