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

        #endregion

        void Start()
        {
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var player = Instantiate(_playerPrefab, _playerSpawnPositions[i]);
                player.GetComponent<Renderer>().material.color = PlayersInfo.Color[i];
            }
        }
        
    }

}
