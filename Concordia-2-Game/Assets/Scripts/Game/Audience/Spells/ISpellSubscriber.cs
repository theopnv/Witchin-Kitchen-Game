using System.Collections;
using System.Collections.Generic;
using con2.game;
using UnityEngine;

public interface ISpellSubscriber
{
    /// <summary>
    /// When this method is called by the Observer, the concerned sub-system
    /// should change its behaviour to react to the spell.
    /// For instance, if PlayerMovement.ActivateSpellMode() is called,
    /// the player must react to some spell
    /// (ie. network ads, inverted controllers...)
    ///
    /// It's up to the subsystem to determine the spell duration.
    /// </summary>
    void ActivateSpellMode();
}

