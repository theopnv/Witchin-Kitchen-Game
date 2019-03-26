using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{

    public class EndGameManager : MonoBehaviour
    {
        public AudienceInteractionManager AudienceInteractionManager;
        private Text m_rematchText, m_winnerText;
        public float REMATCH_TIMER = 15;
        private int MaxNumPlayers = 4;
        private bool m_acceptingInput = false;
        private List<List<PlayerManager>> m_finalRankings;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void Update()
        {
            if (m_acceptingInput)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton0) ||
                    Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    AudienceInteractionManager.SendEndGame(true);
                    SceneManager.LoadScene(SceneNames.Game);
                    Destroy(this);
                }

                if (Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    SceneManager.LoadScene(SceneNames.MainMenu);
                    Destroy(this);
                }
            }
        }

        public void SetFinalRankings(List<List<PlayerManager>> ranks)
        {
            m_finalRankings = ranks;
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;

            SetWinnerText();
            SetAvatars();
            StartCoroutine(BackToMainMenuAfterShortPause());
        }

        private void SetWinnerText()
        {
            m_winnerText = GameObject.Find("WinnerText").GetComponent<Text>();
            m_rematchText = GameObject.Find("RematchText").GetComponent<Text>();
            m_winnerText.text = "";
            foreach (var scoregroup in m_finalRankings)
            {
                foreach (var player in scoregroup)
                {
                    var count = player.CollectedIngredientCount;
                    m_winnerText.text += player.Name + " collected " + count + " ingredient" + (count == 1 ? "" : "s") + "\n\n";
                }
            }
        }

        private void SetAvatars()
        {
            var avatarContainer = GameObject.Find("Avatars").transform;
            int rank = 0;
            int count = 0;
            bool allCharactersHandled = false;
            Debug.Log(m_finalRankings.Count + " ranks");
            for (int i = 0; i < avatarContainer.childCount; i++)
            {
                var a = avatarContainer.GetChild(i);
                if (!allCharactersHandled)
                {
                    //Set color and correct model
                    SkinnedMeshRenderer[] skinRenderer = a.GetComponentsInChildren<SkinnedMeshRenderer>();
                    var id = m_finalRankings[rank][count].ID;
                    skinRenderer[id % 2].material.mainTexture = ColorsManager.Get().PlayerColorTextures[id];
                    skinRenderer[(id + 1) % 2].gameObject.SetActive(false);

                    //Scale based on rank
                    a.gameObject.transform.localScale *= (MaxNumPlayers + 0.5f - rank)/2.0f; //add 0.5 so the player in 4th is shrunk, not set to 0x scale
                    if (rank == 0)
                    {
                        var anim = a.GetComponentInChildren<MenuModelAnims>(true);
                        anim.Carry();
                    }

                    count++;
                    if (count >= m_finalRankings[rank].Count)
                    {
                        count = 0;
                        rank++;
                        if (rank >= m_finalRankings.Count)
                            allCharactersHandled = true;
                    }
                }
                else
                {
                    a.gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator BackToMainMenuAfterShortPause()
        {
            yield return new WaitForSeconds(0.5f);
            m_winnerText.enabled = true;
            yield return new WaitForSeconds(0.5f);
            m_rematchText.enabled = true;
            for (var i = REMATCH_TIMER; i >= 0; i--)
            {
                m_rematchText.text = "Rematch?\n" + i;
                yield return new WaitForSeconds(1);
                if (i == REMATCH_TIMER - 1)
                {
                    m_acceptingInput = true;
                }
            }

            AudienceInteractionManager.SendEndGame(false);
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
