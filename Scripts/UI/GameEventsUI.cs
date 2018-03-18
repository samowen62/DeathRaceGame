using System.Collections.Generic;
using UnityEngine;

//maybe just put this in raceplayer then?
public class GameEventsUI : PausableBehaviour
{
    private Queue<EventTextUI> eventsQueue = new Queue<EventTextUI>();
    private EventTextUI initialEventTextUI;

    // Use this for initialization
    protected override void _awake()
    {
        initialEventTextUI = AppConfig.findOnly<EventTextUI>();
    }

    private void addMessage(string message)
    {
        var eventTextUI = Instantiate(
            initialEventTextUI,
            transform.position,
            Quaternion.identity);
        eventTextUI.transform.parent = transform;
        eventTextUI.setText(message);
        eventTextUI.bumpDown();

        foreach(var eve in eventsQueue.ToArray())
        {
            eve.bumpDown();
        }
        eventsQueue.Enqueue(eventTextUI);

        callAfterSeconds(20, () =>
        {
            var eve = eventsQueue.Dequeue();
            eve.fadeOut();
        });
    }

    public void PlayerDeathMessage(string player)
    {
        addMessage(AppConfig.getRacerDisplayName(player) + " died");
    }

    public void PlayerFinishMessage(string player)
    {
        addMessage(AppConfig.getRacerDisplayName(player) + " Finished");
    }

    public void PlayerKilledMessage(string player, string killedBy)
    {
        addMessage(AppConfig.getRacerDisplayName(player) + " was killed by " + AppConfig.getRacerDisplayName(killedBy));
    }

    public void PlayerLapMessage(string player, int lap)
    {
        addMessage(AppConfig.getRacerDisplayName(player) + " entered lap " + lap);
    }
}
