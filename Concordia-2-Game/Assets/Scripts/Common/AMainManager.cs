using System;
using System.Collections.Generic;
using con2.game;
using UnityEngine;

namespace con2
{
    public abstract class AMainManager : MonoBehaviour
    {
        private Dictionary<int, Tuple<bool, string, Color>> _Players = new Dictionary<int, Tuple<bool, string, Color>>()
        {
            { 0, new Tuple<bool, string, Color>(false, "Gandalf the OG", Color.red) },
            { 1, new Tuple<bool, string, Color>(false, "Sabrina the Tahini Witch", Color.blue) },
            { 2, new Tuple<bool, string, Color>(false, "Snape the Punch-master", Color.green) },
            { 3, new Tuple<bool, string, Color>(false, "Herbione Granger", Color.yellow) },
        };

        public Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();

        public abstract List<IInputConsumer> GetInputConsumers(int playerIndex);

        protected virtual void Update()
        {
            DevMode();
        }

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

        protected void ActivatePlayer(bool activate, int i)
        {
            if (activate)
            {
                if (!_Players[i].Item1)
                {
                    _Players[i] = new Tuple<bool, string, Color>(true, _Players[i].Item2, _Players[i].Item3);
                    PlayersInfo.Name[i] = _Players[i].Item2;
                    PlayersInfo.Color[i] = _Players[i].Item3;
                    ++PlayersInfo.PlayerNumber;
                    GetComponent<ASpawnPlayerController>().InstantiatePlayer(i, OnPlayerInitialized);
                }
            }
            else
            {
                --PlayersInfo.PlayerNumber;
            }
        }

        #region Dev Mode

        void DevMode()
        {
            if (Application.isEditor)
            {
                ActivatePlayersFromKeyboard();
            }
        }

        void ActivatePlayersFromKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivatePlayer(true, 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivatePlayer(true, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivatePlayer(true, 2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ActivatePlayer(true, 3);
            }
        }

        #endregion
    }

}