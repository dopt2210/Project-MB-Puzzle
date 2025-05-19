using UnityEngine;

public class Ending : Action
{
    public override void Act()
    {
        GameManager.Instance.LevelUpgrade();
    }
}
