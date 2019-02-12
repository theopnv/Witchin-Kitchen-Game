using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersController : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform _spawnPositionPlayer1;
        [SerializeField] private Transform _spawnPositionPlayer2;

        [SerializeField] private GameObject _playerPrefab;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            var player1 = Instantiate(_playerPrefab, _spawnPositionPlayer1);
            player1.GetComponent<Renderer>().material.color = PlayersInfo.Color[0];

            var player2 = Instantiate(_playerPrefab, _spawnPositionPlayer2);
            player2.GetComponent<Renderer>().material.color = PlayersInfo.Color[1];
        }
        
    }

}
