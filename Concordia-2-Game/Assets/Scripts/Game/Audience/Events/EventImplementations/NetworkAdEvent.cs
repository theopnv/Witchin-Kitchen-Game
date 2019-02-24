using UnityEngine;
using System.Collections;
using con2;
using con2.game;

public class NetworkAdEvent : AbstractAudienceEvent
{
    public float m_NetworkAdsDuration = 10.0f;

    private Gamepad[] m_playerGamepads;

    void Start()
    {
        SetUpEvent();
    }

    public override Events.EventID GetEventID()
    {
        return Events.EventID.network_ads;
    }

    public override IEnumerator EventImplementation()
    {
        
        m_eventText.text = "The audience spammed you with important ads!";
        
        yield return new WaitForSeconds(m_NetworkAdsDuration);
        
    }
}
