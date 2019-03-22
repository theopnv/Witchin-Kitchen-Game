using System.Collections;
using System.Collections.Generic;
using con2.lobby;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{
    public class RecipeManagerLobby : ARecipeManager
    {
        private LobbyManager m_mgm;

        protected override void Awake()
        {
            base.Awake();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_mgm = managers.GetComponentInChildren<LobbyManager>();
            m_potionSpawner = m_mgm.GetComponent<OnCompletePotion>();
        }

        protected override void NextRecipe()
        {
            m_currentPotionRecipe = new Recipe(GlobalRecipeList.GetNextRecipe(++m_currentRecipeIndex));
            SetNewRecipeUI();
            Owner.CompletedPotionCount = m_currentRecipeIndex;
            m_audienceInteractionManager?.SendGameStateUpdate();
            if (m_currentRecipeIndex > 0)
            {
                var spellManager = FindObjectOfType<SpellsManager>();
                spellManager.LaunchSpellRequest(Owner.ID);
            }
        }

        protected override AMainManager GetMainManager() => m_mgm;
    }


}
