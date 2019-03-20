using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersControllerLobby : ASpawnPlayerController
    {
        #region Private Variables
        
        [SerializeField] private List<Transform> _PlayerZoneSpawnPositions;

        #endregion

        public override void Awake()
        {
            base.Awake();

            // Initialize players
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                InstantiatePlayer(i);
            }
        }
        
        public override Transform GetZoneSpawnPosition(int i)
        {
            return _PlayerZoneSpawnPositions[i];
        }

        public override Vector3 GetPlayerSpawnPositionInZone(int i)
        {
            var pos = new Vector3
            {
                x = _PlayerZoneSpawnPositions[i].transform.position.x + 1,
                y = 0,
                z = _PlayerZoneSpawnPositions[i].transform.position.z + 1,
            };
            return pos;
        }
    }
}
