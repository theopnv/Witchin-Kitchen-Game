using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersControllerGame : ASpawnPlayerController
    {
        #region Private Variables

        private List<Transform> _PlayerZoneSpawnPositions;

        [SerializeField] private float _PlayerDistanceFromCenter = 2f;
        #endregion

        public void Start()
        {
            PrepareSpawnZone();
        }

        private void PrepareSpawnZone()
        {
            var playersShiftMagicVar = GameInfo.PlayerNumber == 2
                ? 180
                : GameInfo.PlayerNumber == 3
                    ? 150
                    : 135;

            var increment = 360 / (GameInfo.PlayerNumber != 0 ? GameInfo.PlayerNumber : 1);
            _PlayerZoneSpawnPositions = new List<Transform>();
            for (var i = 0; i < GameInfo.PlayerNumber; i++)
            {
                var radians = (increment * i + playersShiftMagicVar) * Mathf.Deg2Rad;
                var pos = new Vector3()
                {
                    x = Mathf.Cos(radians),
                    y = 0,
                    z = Mathf.Sin(radians),
                };
                pos *= _KitchensDistanceFromCenter;
                var spawnPoint = Instantiate(_SpawnPointPrefab, pos, Quaternion.identity);
                _PlayerZoneSpawnPositions.Add(spawnPoint.transform);
            }
        }

        public override Transform GetZoneSpawnPosition(int i)
        {
            if (_PlayerZoneSpawnPositions == null || _PlayerZoneSpawnPositions.Count == 0)
            {
                PrepareSpawnZone();
            }
            return _PlayerZoneSpawnPositions[i];
        }

        public override Vector3 GetPlayerSpawnPositionInZone(int i)
        {
            var playersShiftMagicVar = GameInfo.PlayerNumber == 2
                ? 180
                : GameInfo.PlayerNumber == 3
                    ? 150
                    : 135;

            var increment = 360 / (GameInfo.PlayerNumber != 0 ? GameInfo.PlayerNumber : 1);
            var radians = (increment * i + playersShiftMagicVar) * Mathf.Deg2Rad;
            var pos = new Vector3()
            {
                x = Mathf.Cos(radians),
                y = 0,
                z = Mathf.Sin(radians),
            };
            pos *= _PlayerDistanceFromCenter;

            return pos;
        }

    }
}
