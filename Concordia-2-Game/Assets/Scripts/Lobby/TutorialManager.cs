using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.main_menu;
using con2.messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace con2.lobby
{

    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private LobbyManager _LobbyManager;
        [SerializeField] private TextMeshProUGUI _CurrentInstruction;
        [SerializeField] private ItemSpawner _ItemSpawner;
        [SerializeField] private SpellsManager _SpellsManager;
        [SerializeField] private EventManager _EventsManager;

        private int _PlayersProgression = 1;
        private int _CurrentInstructionIdx;
        private Dictionary<int, string> _Instructions = new Dictionary<int, string>()
        {
            { 0, "Welcome to Witchin' Kitchen, competitors!\r\nToday's crazy show is broadcasted all over the world. \r\nCompete to prove that you're the best witch or wizard!" },
            { 1, "You score points by making potions in your cauldron.\r\nGrab the ingredient you see in your recipe\r\nand then throw it in your cauldron."},
            { 2, "Once an ingredient is in your cauldron,\r\n you'll have to process it.\r\nStand close to the cauldron and do the prompted action."},
            { 3, "You scored one point!\r\nWhen a potion is made, an audience member gets to drink it to casts a spell on one of you. Watch out!"},
            { 4, "Oh no! Audience members cast the spell\r\nDisco Mania on all of you.\r\nYour controls are inverted during the spell!"},
            { 5, "To win, you must fight your opponents to get ingredients before they do.\r\nYou have the short-ranged slap attack [B] with a brief cooldown\r\nand the long-ranged fireballs [X] with a longer cooldown."},
            { 6, "You can tell your fireball is charged when you see a mini\r\nfireball spinning around your character. Throw a fireball\r\n at one of your opponents to ready up!"},
        };

        private void LevelUpPlayersProgression()
        {
            _CurrentInstructionIdx = ++_PlayersProgression;
            _CurrentInstruction.text = _Instructions[_CurrentInstructionIdx];
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
            var recipeManagers = FindObjectsOfType<RecipeManagerLobby>();
            var recipeManager = recipeManagers.FirstOrDefault(r => r.Owner.ID == player.ID);
            if (recipeManager)
            {
                recipeManager.OnIngredientAdded += _0_IngredientCollected;
                recipeManager.OnCompletedPotion += _1_CompletedPotion;
            }

            var fireballManager = player.GetComponentInChildren<PlayerFireball>();
            fireballManager.OnFireballCasted += () => OnFireBallCasted(player.ID);

            player.PlayerHUD.transform.SetSiblingIndex(player.ID);
        }

        #region Instructions

        private IEnumerator Welcome()
        {
            yield return new WaitForSeconds(10); // TODO set back to 10
            yield return Potion();
        }

        private IEnumerator Potion()
        {
            foreach (var player in _LobbyManager.PlayersInstances)
            {
                var pointer = player.Value.PlayerHUD.transform.Find("Pointer");
                pointer.gameObject.SetActive(true);
            }
            ++_CurrentInstructionIdx;
            _CurrentInstruction.text = _Instructions[_CurrentInstructionIdx];
            _ItemSpawner.SpawnableItems[game.Ingredient.MUSHROOM].AskToInstantiate();
            _ItemSpawner.SpawnableItems[game.Ingredient.MUSHROOM].MaxNbOfInstances = 0;
            yield return null;
        }

        private bool ingredientGate = false;
        private void _0_IngredientCollected(int playerIdx)
        {
            if (!ingredientGate)
            {
                ingredientGate = true;
                LevelUpPlayersProgression();

                foreach (var player in _LobbyManager.PlayersInstances)
                {
                    var pointer = player.Value.PlayerHUD.transform.Find("Pointer");
                    pointer.gameObject.SetActive(false);
                }
            }
        }

        public Image AudiencePointer;
        private bool potionGate = false;
        private void _1_CompletedPotion(int playerIdx)
        {
            if (!potionGate)
            {
                potionGate = true;
                LevelUpPlayersProgression();
                StartCoroutine(_2_CastSpells());
                AudiencePointer.gameObject.SetActive(true);
                _LobbyManager.PlayersInstances[playerIdx].PlayerHUD.transform.Find("Organizer/Score/ScorePointer").gameObject.SetActive(true);
            }
        }

        private IEnumerator _2_CastSpells()
        {
            yield return new WaitForSeconds(10);
            LevelUpPlayersProgression();
            AudiencePointer.gameObject.SetActive(false);

            foreach (var player in _LobbyManager.PlayersInstances)
            {
                player.Value.PlayerHUD.transform.Find("Organizer/Score/ScorePointer").gameObject.SetActive(false);
                _SpellsManager.OnSpellCasted(new Spell()
                {
                    caster = null,
                    spellId = (int)Spells.SpellID.disco_mania,
                    targetedPlayer = new messages.Player()
                    {
                        id = player.Value.ID,
                    }
                });
            }

            yield return _3_FireballPunch();
        }

        private IEnumerator _3_FireballPunch()
        {
            yield return new WaitForSeconds(7);
            _ItemSpawner.SpawnableItems[game.Ingredient.MUSHROOM].MaxNbOfInstances = 1;
            _ItemSpawner.SpawnableItems[game.Ingredient.MUSHROOM].AskToInstantiate();
            LevelUpPlayersProgression();
            yield return new WaitForSeconds(10);
            LevelUpPlayersProgression();
        }

        private void OnFireBallCasted(int playerIdx)
        {
            if (_PlayersProgression == _Instructions.Count - 1)
            {
                _LobbyManager.OnFireballCasted(playerIdx);
            }
        }

        private IEnumerator EndTutorial()
        {
            yield return _LobbyManager.StartGame();
        }

        #endregion
    }

}
