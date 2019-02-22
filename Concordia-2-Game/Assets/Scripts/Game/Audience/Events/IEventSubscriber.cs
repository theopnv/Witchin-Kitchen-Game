using System.Collections;
using System.Collections.Generic;
using con2.game;
using UnityEngine;

public interface IEventSubscriber
{
    /// <summary>
    /// When this method is called by the Observer, the concerned sub-system
    /// should change its behaviour to react to the event.
    /// For instance, if PlayerMovement.ActivateEventMode() is called,
    /// the player must react to some event
    /// (be it slippery floors, inverted controllers...)
    ///
    /// It's up to the subsystem to determine the event duration.
    /// </summary>
    void ActivateEventMode();
}
