using UnityEngine;
using System.Collections;
using con2;
using con2.game;

public class NetworkAdEvent : AbstractAudienceEvent
{
    public float m_NetworkAdsDuration = 10.0f;

    private Gamepad[] m_playerGamepads;

    [SerializeField] private GameObject _NetworkAdsCanvasPrefab;
    private GameObject _NetworkAdsCanvasInstance;

    void Start()
    {
        SetUpEvent();
    }

    public override void EventStart()
    {}

    public override IEnumerator EventImplementation()
    {
        _NetworkAdsCanvasInstance = Instantiate(_NetworkAdsCanvasPrefab);
        _MessageFeedManager.AddMessageToFeed("This is ad time...", MessageFeedManager.MessageType.arena_event);
        
        yield return new WaitForSeconds(m_NetworkAdsDuration);
        Destroy(_NetworkAdsCanvasInstance);
    }

    public override Events.EventID GetEventID()
    {
        return Events.EventID.network_ads;
    }
}
