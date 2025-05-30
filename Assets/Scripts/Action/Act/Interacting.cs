using UnityEngine;

public class Interacting : Action
{
    [SerializeField] private MazeObjects obj;
    public override void Awake()
    {
        base.Awake();
        obj = GetComponentInParent<MazeObjects>();
    }
    public override void Act()
    {
        obj.IsInRange = true;
    }
    public override void CancelAct()
    {
        obj.IsInRange = false;
    }
}
