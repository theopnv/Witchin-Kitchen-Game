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

        private static EndGameManager instance = null;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                return;
            }
            Destroy(gameObject);
        }


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
                    StartCoroutine(Transition.Get().SequenceIn(null, _LoadGame()));
                }

                if (Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    StartCoroutine(Transition.Get().SequenceIn(null, _LoadMainMenu()));
                }
            }
        }

        protected IEnumerator _LoadGame()
        {
            AudienceInteractionManager.SendEndGame(true);
            SceneManager.LoadScene(SceneNames.Game);
            Destroy(this);
            yield return null;
        }

        protected IEnumerator _LoadMainMenu()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
            Destroy(this);
            yield return null;
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
                    m_winnerText.text += player.Name + " got " + count + " ingredient" + (count == 1 ? "" : "s") + "\n\n";
                }
            }
        }

        private void SetAvatars()
        {
            var avatarContainer = GameObject.Find("Avatars").transform;
            int rank = 0;
            int count = 0;
            bool allCharactersHandled = false;
            for (int i = 0; i < avatarContainer.childCount; i++)
            {
                var a = avatarContainer.GetChild(i);
                if (!allCharactersHandled)
                {
                    //Set color and correct model, ignore hair renderers
                    SkinnedMeshRenderer[] tempRenderers = a.GetComponentsInChildren<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer[] skinRenderers = new SkinnedMeshRenderer[2];
                    int j = 0;
                    foreach (var r in tempRenderers)
                    {
                        if (r.gameObject.layer != 15)
                        {
                            skinRenderers[j] = r;
                            j++;
                        }
                    }

                    var id = m_finalRankings[rank][count].ID;
                    var model = skinRenderers[id % 2];
                    model.material.mainTexture = ColorsManager.Get().PlayerColorTextures[id];
                    var temp = skinRenderers[(id + 1) % 2].transform;
                    while (temp.parent != null)
                    {
                        if (temp.parent.tag == Tags.PLAYER_TAG)
                        {
                            temp.parent.gameObject.SetActive(false);
                            break;
                        }
                        temp = temp.parent.transform;
                    }

                    //Scale based on rank
                    a.gameObject.transform.localScale *= (MaxNumPlayers + 0.5f - rank)/2.0f; //add 0.5 so the player in 4th is shrunk, not set to 0x scale
                    a.gameObject.transform.position = new Vector3(0, 0, i*60 - 240);
                    if (rank == 0)
                    {
                        var anim = model.GetComponentInParent<MenuModelAnims>();
                        anim.Carry();
                        var trophy = a.transform.Find("trophy");
                        trophy.gameObject.SetActive(true);
                    }
                    else if ( rank == m_finalRankings.Count - 1)
                    {
                        var anim = model.GetComponentInParent<MenuModelAnims>();
                        anim.Dizzy();
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
            m_winnerText.enabled = true;
            m_rematchText.enabled = true;
            for (var i = REMATCH_TIMER; i > 0; i--)
            {
                m_rematchText.text = "Rematch?\n" + i;
                yield return new WaitForSeconds(1);
                if (i == REMATCH_TIMER - 1)
                {
                    m_acceptingInput = true;
                }
            }

            StartCoroutine(Transition.Get().SequenceIn(null, _BackToMainMenu()));
        }

        protected IEnumerator _BackToMainMenu()
        {
            AudienceInteractionManager.SendEndGame(false);
            SceneManager.LoadScene(SceneNames.MainMenu);
            yield return null;
        }
    }
}
