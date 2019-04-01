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
        [SerializeField] private GameObject _TutorialMushroomPrefab;
        [SerializeField] private GameObject[] _FirstItemSpawnerLocations = new GameObject[4];
        [SerializeField] private ItemSpawner _ItemSpawner;
        [SerializeField] private GameObject _Dividers;
        [SerializeField] private SpellsManager _SpellsManager;
        [SerializeField] private EventManager _EventsManager;

        private int _PlayersProgression = 1;
        private Dictionary<int, bool> _PlayersPotions = new Dictionary<int, bool>();
        private int _CurrentInstructionIdx;
        private Dictionary<int, string> _Instructions = new Dictionary<int, string>()
        {
            { 0, "Welcome to Witchin' Kitchen, competitors!\r\nCompete to prove that you're the best witch or wizard!" },
            { 1, "Score points by making potions.\r\nFollow the recipe in your bottom HUD.\r\nGrab and Throw <sprite=0> an ingredient in your cauldron."},
            { 2, "Stand near your cauldron and do the prompted action to process the ingredient."},
            { 3, "You each scored one point!\r\nWhen a potion is made, an audience member\r\nwill cast a spell on one of you. Watch out!"},
            { 4, "Oh no! The audience members cast the spell\r\nDisco Mania on each of you.\r\nYour controls are inverted during the spell!"},
            { 5, "Get ingredients from your enemies by attacking them with your\r\nshort-ranged slap attack <sprite=1> (brief cooldown),\r\nand long-ranged fireballs <sprite=2> (longer cooldown)."},
            { 6, "A mini fireball spinning around you means it is charged.\r\nNow, launch a fireball <sprite=2> to ready up!"},
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

            Debug.Log(player.ID);
            var players = _LobbyManager.PlayersInstances;
            foreach (var p in players)
            {
                p.Value.PlayerHUD.transform.SetSiblingIndex(p.Value.ID);
            }

            if (!_PlayersPotions.ContainsKey(player.ID))
            {
                _PlayersPotions.Add(player.ID, false);
            }

            if (_CurrentInstructionIdx == 1 || _CurrentInstructionIdx == 2)
            {
                Instantiate(_TutorialMushroomPrefab, _FirstItemSpawnerLocations[player.ID].transform.position, Quaternion.identity);
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
            foreach (var player in _LobbyManager.PlayersInstances)
            {
                var pointer = player.Value.PlayerHUD.transform.Find("Pointer");
                pointer.gameObject.SetActive(true);
            }
            ++_CurrentInstructionIdx;
            _CurrentInstruction.text = _Instructions[_CurrentInstructionIdx];

            int i = 0;
            foreach (var spawner in _FirstItemSpawnerLocations)
            {
                if (_LobbyManager.PlayersInstances.ContainsKey(i))
                {
                    Instantiate(_TutorialMushroomPrefab, spawner.transform.position, Quaternion.identity);
                }
                i++;
            }

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
        private bool potionGate = false, thisStepDone = false;  //guuuhhhh
        private void _1_CompletedPotion(int playerIdx)
        {
            _PlayersPotions[playerIdx] = true;
            potionGate = true;
            foreach (var potionStatus in _PlayersPotions)   //Assuming that all players will be in at this point, anyone who joins after doesn't get to make a potion
            {
                if (potionStatus.Value == false)
                    potionGate = false;
            }

            if (potionGate && !thisStepDone)
            {
                thisStepDone = true;
                LevelUpPlayersProgression();
                StartCoroutine(_2_CastSpells());
                AudiencePointer.gameObject.SetActive(true);
                foreach (var player in _LobbyManager.PlayersInstances)
                {
                    player.Value.PlayerHUD.transform.Find("Organizer/Score/ScorePointer").gameObject.SetActive(true);
                }
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
            yield return new WaitForSeconds(10);
            StartCoroutine(RemoveDividers());
            yield return new WaitForSeconds(1);
            _ItemSpawner.SpawnableItems[game.Ingredient.MUSHROOM].MaxNbOfInstances = 1;
            _ItemSpawner.SpawnableItems[game.Ingredient.MUSHROOM].AskToInstantiate();
            LevelUpPlayersProgression();
            yield return new WaitForSeconds(13);
            LevelUpPlayersProgression();
        }

        private IEnumerator RemoveDividers()
        {
            var dividerVisuals = _Dividers.GetComponentsInChildren<SpriteRenderer>();
            var startTime = Time.time;
            var startColor = dividerVisuals[0].color;
            while (Time.time - startTime < 1.5f)
            {
                var newAlpha = Mathf.Lerp(1.0f, 0.0f, Time.time - startTime);
                foreach (var line in dividerVisuals)
                {
                    line.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                }
                yield return new WaitForEndOfFrame();
            }
            _LobbyManager.TempInvisibleWalls.SetActive(false);
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
