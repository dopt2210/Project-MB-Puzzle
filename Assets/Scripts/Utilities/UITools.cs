
using UnityEngine.EventSystems;
using UnityEngine;

public static class UITools
{
    public static void AddEventTrigger(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public static bool ValidValue(MazeSO mazeSO)
    {

        int fixedAxes = (mazeSO.Width == 1 ? 1 : 0) +
                (mazeSO.Height == 1 ? 1 : 0) +
                (mazeSO.Depth == 1 ? 1 : 0);

        if (fixedAxes != 1)
        {
            Debug.Log("Maze must have exactly one fixed axis.");
            return false;
        }

        if (mazeSO.Width < 1 || mazeSO.Height < 1 || mazeSO.Depth < 1)
        {
            Debug.Log("All axes must be at least 1.");
            return false;
        }
        return true;
    }
}
