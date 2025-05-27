public class FittingObject : Action
{
    CompareObject CompareObject;
    public override void Awake()
    {
        base.Awake();
        CompareObject = GetComponentInParent<CompareObject>();
    }
    public override void Act()
    {
        CompareObject.ObjectFit(triggerActionCtrl.triggerAndAction.Keys);
    }
    public override void CancelAct()
    {
        CompareObject.ObjectNotFit(triggerActionCtrl.triggerAndAction.Keys);

    }
}
