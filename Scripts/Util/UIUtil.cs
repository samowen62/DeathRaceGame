using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class UIUtil {

    public static void addTrigger(UnityAction action, EventTriggerType triggerType, Button button, GameObject gameObject)
    {
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();

        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((eventData) => { action(); });

        EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

        eventTrigger.triggers.Add(entry);
    }
}
