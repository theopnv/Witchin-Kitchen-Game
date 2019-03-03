using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class DiscoManiaSpell : ASpell
    {
        public float m_DiscoManiaDuration = 10.0f;
        public GameObject m_discoBallPrefab;

        // Start is called before the first frame update

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            Vector3 startExitPos = new Vector3 (20, 5, 20);

            GameObject discoball = Instantiate(m_discoBallPrefab, startExitPos, new Quaternion(0, 0, 0, 0));
            DiscoBall discoballController = discoball.GetComponent<DiscoBall>();
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            GameObject targetPlayer = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayers()[_TargetedPlayer.id];
            discoballController.FollowTarget = targetPlayer;

            var playerGamepad = GamepadMgr.Pad(_TargetedPlayer.id);
            playerGamepad.InvertMovement();


            yield return new WaitForSeconds(m_DiscoManiaDuration);

            playerGamepad.InvertMovement();

            GameObject leaveTarget = Instantiate(new GameObject(), startExitPos, new Quaternion(0, 0, 0, 0));
            discoballController.FollowTarget = leaveTarget;
            yield return new WaitForSeconds(3);     //Allow the discoball to fly away like when it entered
            GameObject.Destroy(discoball);
            GameObject.Destroy(leaveTarget);

        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.disco_mania;
        }
    }

}
