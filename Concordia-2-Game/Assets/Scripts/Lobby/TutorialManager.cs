using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using TMPro;
using UnityEngine;

namespace con2.lobby
{

    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private LobbyManager _LobbyManager;
        [SerializeField] private TextMeshProUGUI _CurrentInstruction;
        [SerializeField] private ItemSpawner _ItemSpawner;

        private RecipeManagerLobby _RecipeManager;
        private int _CurrentStep;

        public void Start()
        {
            _CurrentStep = 1;
            _CurrentInstruction.text = "Welcome to Witchin' Kitchen, candidates!\r\nToday's crazy show is broadcasted all over the world. It's time to show that you're the best witch or wizard!";
        }

        public void Run()
        {
            StartCoroutine(_1_Welcome());
        }

        #region Instructions

        private IEnumerator _1_Welcome()
        {
            yield return new WaitForSeconds(10);
            yield return _2_Goal();
        }

        private IEnumerator _2_Goal()
        {
            _CurrentStep = 2;
            var lowestPlayerIndx = _LobbyManager.PlayersInstances.Keys.Min();
            var recipeManagers = FindObjectsOfType<RecipeManagerLobby>();
            _RecipeManager = recipeManagers.First(r => r.Owner.ID == lowestPlayerIndx);
            _RecipeManager.OnProcessedIngredient += _2_Completed;
            _RecipeManager.OnCompletedPotion += _3_Completed;
            _CurrentInstruction.text = "Your goal is to complete potions. Your cauldron will be your most precious ally.\r\nTry to grab this ingredient over there and to throw it into!";
            _ItemSpawner.SpawnableItems[Ingredient.MUSHROOM].AskToInstantiate();
            yield return null;
        }

        private void _2_Completed()
        {
            _CurrentInstruction.text = "Congrats! You just processed your first ingredient.\r\nThe remaining ones are shown in your HUD. Go get them to finish a potion!";
        }

        private void _3_Completed()
        {
            _CurrentInstruction.text = "You scored one point! At each potion, the audience gets to cast a special spell on one of you. Get ready!";
        }

        #endregion
    }

}
