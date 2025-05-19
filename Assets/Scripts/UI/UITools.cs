using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
    #region event
    public static IEnumerator DelayedAction(System.Action action)
    {
        SoundManager.Instance.PlaySound2D("Click");
        Debug.Log("invoke");
        yield return new WaitForSeconds(0.15f);
        action?.Invoke();
    }
    public static void OnPointerEnter(PointerEnterEvent data)
    {
        SoundManager.Instance.PlaySound2D("Hover");
    }
    public static void OnPointerEnter(FocusInEvent data)
    {
        SoundManager.Instance.PlaySound2D("Hover");
    }
    public static void OnPointerClick(PointerDownEvent data)
    {
        SoundManager.Instance.PlaySound2D("Click");
    }  
    public static void OnPointerClick(ClickEvent data)
    {
        SoundManager.Instance.PlaySound2D("Click");
    }
    public static void OnPointerClick(KeyDownEvent data)
    {
        SoundManager.Instance.PlaySound2D("Click");
    }
    public static void UpdateMusicVolume(float volume, AudioMixer audioMixer)
    {
        float newVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", volume);
    }
    public static void UpdateSFXVolume(float volume, AudioMixer audioMixer)
    {
        float newVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", volume);
    }
    #endregion
}
