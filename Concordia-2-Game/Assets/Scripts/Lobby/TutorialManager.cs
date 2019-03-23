using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.main_menu;
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

        private int _CurrentInstructionIdx;
        private Dictionary<int, string> _Instructions = new Dictionary<int, string>()
        {
            { 0, "Welcome to Witchin' Kitchen, candidates!\r\nToday's crazy show is broadcasted all over the world. It's time to show that you're the best witch or wizard!" },
            { 1, "Your goal is to complete potions. Your cauldron will be your most precious ally.\r\nTry to grab this ingredient over there and to throw it into!"},
            { 2, "You scored one point! At each potion, the audience gets to cast a spell on one of you. Get ready!"},
            { 3, "Oh oh! The audience casted Disco Mania on you.\r\nYour controls are inverted for 10 seconds."},
            { 4, "Two weapons are at your disposal in the arena:\r\nHit [B] to punch your opponents and [Right Trigger] to throw fireballs."},
            { 5, "Hit [B] to punch your opponents and [Right Trigger] to throw fireballs.\r\nThrow a fireball at one of your opponents to launch the game!"},
        };

        private int GetMaxPlayerIndx()
        {
            return _PlayersProgression.OrderByDescending(p => p.Value).First().Key;
        }

        private void LevelUpPlayersProgression(int playerIdx)
        {
            ++_PlayersProgression[playerIdx];
            if (_PlayersProgression[playerIdx] > 5)
            {
                _PlayersProgression.Remove(playerIdx);
            }

            if (_PlayersProgression.Count > 0)
            {
                _CurrentInstructionIdx = _PlayersProgression[GetMaxPlayerIndx()];
                _CurrentInstruction.text = _Instructions[_CurrentInstructionIdx];
            }
            else
            {
                StartCoroutine(EndTutorial());
            }
        }

        public void Start()
        {
            if (MenuInfo.DoTutorial)
            {
                _CurrentInstruction.transform.parent.gameObject.SetActive(true);
                _CurrentInstruction.text = _Instructions[_CurrentInstructionIdx];
            }
        }

        public void Run()
        {
            StartCoroutine(Welcome());
        }

        public void OnPlayerInitialized(PlayerManager player)
        {
            if (!_PlayersProgression.ContainsKey(player.ID))
            {
                _PlayersProgression.Add(player.ID, 1);
            }
            var recipeManagers = FindObjectsOfType<RecipeManagerLobby>();
            var recipeManager = recipeManagers.FirstOrDefault(r => r.Owner.ID == player.ID);
            if (recipeManager)
            {
                recipeManager.OnCompletedPotion += _1_CompletedPotion;
            }

            var fireballManager = player.GetComponentInChildren<PlayerFireball>();
            fireballManager.OnFireballCasted += () => OnFireBallCasted(player.ID);
        }

        #region Instructions

        private IEnumerator Welcome()
        {
            yield return new WaitForSeconds(10);
            yield return Potion();
        }

        private IEnumerator Potion()
        {
            ++_CurrentInstructionIdx;
            _CurrentInstruction.text = _Instructions[_CurrentInstructionIdx];
            _ItemSpawner.SpawnableItems[Ingredient.MUSHROOM].AskToInstantiate();
            yield return null;
        }

        private void _1_CompletedPotion(int playerIdx)
        {
            if (_PlayersProgression[playerIdx] <= 1)
            {
                LevelUpPlayersProgression(playerIdx);
                StartCoroutine(_2_CastSpells(playerIdx));
            }
        }

        private IEnumerator _2_CastSpells(int playerIdx)
        {
            yield return new WaitForSeconds(10);
            LevelUpPlayersProgression(playerIdx);

            _SpellsManager.OnSpellCasted(new Spell()
            {
                caster = null,
                spellId = (int)Spells.SpellID.disco_mania,
                targetedPlayer = new messages.Player()
                {
                    id = playerIdx,
                }
            });

            yield return _3_FireballPunch(playerIdx);
        }

        private IEnumerator _3_FireballPunch(int playerIdx)
        {
            yield return new WaitForSeconds(10);
            LevelUpPlayersProgression(playerIdx);
            yield return new WaitForSeconds(10);
            LevelUpPlayersProgression(playerIdx);
        }

        private void OnFireBallCasted(int playerIdx)
        {
            if (_PlayersProgression[playerIdx] >= 5)
            {
                LevelUpPlayersProgression(playerIdx);
                _LobbyManager.PlayersInstances[playerIdx].PlayerHUD.SetReadyActive();
            }
        }

        private IEnumerator EndTutorial()
        {
            yield return _LobbyManager.StartGame();
        }

        #endregion
    }

}
