using System.Collections.Generic;
using con2.game;
using UnityEngine;

namespace con2
{
    public abstract class AMainManager : MonoBehaviour
    {
        public Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();

        public abstract List<IInputConsumer> GetInputConsumers(int playerIndex);

        public void OnPlayerInitialized(PlayerManager player)
        {
            if (!Players.ContainsKey(player.ID))
            {
                Players.Add(player.ID, player);
            }
            // Reassign Game Context, with newly added input consumers of the player
            // TODO: Optimize this if possible
            FindObjectOfType<InputContextSwitcher>().SetToGameContext(player.ID);
        }
    }

}