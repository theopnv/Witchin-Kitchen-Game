using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersController : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform[] _playerSpawnPositions;

        [SerializeField] private GameObject _playerPrefab;

        private GameObject[] m_players = new GameObject[PlayersInfo.PlayerNumber];

        #endregion

        void Start()
        {
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            GlobalFightState gfs = managers.GetComponentInChildren<GlobalFightState>();

            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var player = Instantiate(_playerPrefab, _playerSpawnPositions[i]);
                player.name = "Player " + (i + 1);
                player.GetComponent<Renderer>().material.color = PlayersInfo.Color[i];
                player.GetComponent<PlayerInputController>().SetPlayerIndex(i);
                m_players[i] = player;
                gfs.AddFighter(player);
            }

            //Switch control context to game once players are spawned
            InputContextSwitcher contextSwitcher = managers.GetComponentInChildren<InputContextSwitcher>();
            contextSwitcher.SetToGameContext();

            //Initialize kitchens
            GameObject[] kitchens = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
            foreach (GameObject kitchen in kitchens)
            {
                KitchenManager km = kitchen.GetComponent<KitchenManager>();
                km.SetOwner(this);
            }
        }

        public GameObject[] GetPlayers()
        {
            return m_players;
        }

        public GameObject GetPlayerByID(int ID)
        {
            return m_players[ID];
        }
    }
}
