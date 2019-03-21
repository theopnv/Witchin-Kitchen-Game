using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public abstract class AMainManager : MonoBehaviour
    {
        public abstract List<IInputConsumer> GetInputConsumers(int playerIndex);

        public void OnPlayerInitialized()
        {
            // Reassign Game Context, with newly added input consumers of the player
            // TODO: Optimize this if possible
            FindObjectOfType<InputContextSwitcher>().SetToGameContext();
        }
    }

}