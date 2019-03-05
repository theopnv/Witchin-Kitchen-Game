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
            var startExitPos = new Vector3 (20, 5, 20);

            var discoball = Instantiate(m_discoBallPrefab, startExitPos, new Quaternion(0, 0, 0, 0));
            var discoballController = discoball.GetComponent<DiscoBall>();
            var targetPlayer = Players.GetPlayerByID(_TargetedPlayer.id);
            discoballController.FollowTarget = targetPlayer.gameObject;

            var playerMovement = targetPlayer.GetComponent<PlayerMovement>();
            playerMovement.InvertMovement();


            yield return new WaitForSeconds(m_DiscoManiaDuration);

            playerMovement.InvertMovement();

            GameObject leaveTarget = Instantiate(new GameObject(), startExitPos, new Quaternion(0, 0, 0, 0));
            discoballController.FollowTarget = leaveTarget;
            yield return new WaitForSeconds(3);     //Allow the discoball to fly away like when it entered
            Destroy(discoball);
            Destroy(leaveTarget);

        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.disco_mania;
        }
    }

}
