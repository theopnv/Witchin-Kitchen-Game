using System.Collections;
using System.Collections.Generic;
using con2.game;
using UnityEngine;

namespace con2.game
{

    /// <summary>
    /// This is just an example showcasing the use of the event manager
    /// TODO: Remove when not needed anymore
    /// </summary>
    public class FakeEventSubscriber : MonoBehaviour, IEventSubscriber
    {
        void Start()
        {
            var eventManager = gameObject.GetComponent<EventManager>();
            eventManager.AddSubscriber(Events.EventID.freezing_rain, this);
        }

        public void ActivateEventMode()
        {
            Debug.Log("Mmmh, Diz freezing rain iz unfortunate event !!");
        }
    }

}
