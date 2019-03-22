using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.messages;
using TMPro;
using UnityEngine;

namespace con2.lobby
{

    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private LobbyManager _LobbyManager;
        [SerializeField] private TextMeshProUGUI _CurrentInstruction;
        [SerializeField] private ItemSpawner _ItemSpawner;
        [SerializeField] private SpellsManager _SpellsManager;
        [SerializeField] private EventManager _EventsManager;

        private Dictionary<int, int> _PlayersProgression = new Dictionary<int, int>();

        public void Start()
        {
            _CurrentInstruction.text = "Welcome to Witchin' Kitchen, candidates!\r\nToday's crazy show is broadcasted all over the world. It's time to show that you're the best witch or wizard!";
        }

        public void Run()
        {
            StartCoroutine(Welcome());
        }

        public void OnPlayerInitialized(PlayerManager player)
        {
            if (!_PlayersProgression.ContainsKey(player.ID))
            {
                _PlayersProgression.Add(player.ID, 0);
            }
            var recipeManagers = FindObjectsOfType<RecipeManagerLobby>();
            var recipeManager = recipeManagers.FirstOrDefault(r => r.Owner.ID == player.ID);
            if (recipeManager)
            {
                recipeManager.OnCompletedPotion += _1_CompletedPotion;
            }
        }

        #region Instructions

        private IEnumerator Welcome()
        {
            yield return new WaitForSeconds(10);
            yield return Potion();
        }

        private IEnumerator Potion()
        {
            _CurrentInstruction.text = "Your goal is to complete potions. Your cauldron will be your most precious ally.\r\nTry to grab this ingredient over there and to throw it into!";
            _ItemSpawner.SpawnableItems[Ingredient.MUSHROOM].AskToInstantiate();
            yield return null;
        }

        private void _1_CompletedPotion(int playerIdx)
        {
            if (_PlayersProgression.Values.Max() <= 0)
            {
                _CurrentInstruction.text = "You scored one point! At each potion, the audience gets to cast a spell on one of you. Get ready!";
            }

            if (_PlayersProgression[playerIdx] <= 0)
            {
                _PlayersProgression[playerIdx] = 1;
                StartCoroutine(_2_CastSpells(playerIdx));
            }
        }

        private IEnumerator _2_CastSpells(int playerIdx)
        {
            yield return new WaitForSeconds(10);
            _CurrentInstruction.text = "Oh oh! The audience casted Disco Mania on you.\r\nYour controls are inverted for 10 seconds.";

            _SpellsManager.OnSpellCasted(new Spell()
            {
                caster = null,
                spellId = (int)Spells.SpellID.disco_mania,
                targetedPlayer = new messages.Player()
                {
                    id = playerIdx,
                }
            });
            _PlayersProgression[playerIdx] = 2;

            yield return _3_FireballPunch(playerIdx);
        }

        private IEnumerator _3_FireballPunch(int playerIdx)
        {
            yield return new WaitForSeconds(10);
            _CurrentInstruction.text = "Two weapons are at your disposal in the arena:\r\nHit [B] to punch your opponents and [Right Trigger] to throw fireballs.";
            yield return new WaitForSeconds(10);
            _CurrentInstruction.text =
                "Hit [B] to punch your opponents and [Right Trigger] to throw fireballs.\r\nThrow a fireball at one of your opponents to launch the game!";
            _PlayersProgression[playerIdx] = 3;
        }

        #endregion
    }

}
