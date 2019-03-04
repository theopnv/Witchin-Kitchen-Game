using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersController : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform[] _playerSpawnPositions;

        [SerializeField] private GameObject _playerPrefab;

        private Dictionary<int, PlayerManager> m_players;

        #endregion

        void Start()
        {
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);

            // Initialize players
            m_players = new Dictionary<int, PlayerManager>(PlayersInfo.PlayerNumber);
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var player = Instantiate(_playerPrefab, _playerSpawnPositions[i]);
                var playerManager = player.GetComponent<PlayerManager>();

                playerManager.ID = i;
                playerManager.Name = "Player " + playerManager.ID;
                playerManager.Color = PlayersInfo.Color[playerManager.ID];
                m_players[i] = playerManager;
            }

            //Switch control context to game once players are spawned
            var contextSwitcher = managers.GetComponentInChildren<InputContextSwitcher>();
            contextSwitcher.SetToGameContext();

            //Initialize kitchens
            var kitchens = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
            foreach (var kitchen in kitchens)
            {
                var km = kitchen.GetComponent<KitchenManager>();
                km.SetOwner(this);
            }

            //Inform Camera of which players to track
            var camera = GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA);
            camera.GetComponent<SharedCamera>()
                .SetPlayers(m_players
                    .Select(p => p.Value.gameObject)
                    .ToArray());
        }

        public Dictionary<int, PlayerManager> GetPlayers()
        {
            return m_players;
        }

        public PlayerManager GetPlayerByID(int ID)
        {
            return m_players[ID];
        }
    }
}
