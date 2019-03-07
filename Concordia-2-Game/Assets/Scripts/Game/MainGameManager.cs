using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{

    public class MainGameManager : MonoBehaviour, IInputConsumer
    {
        private AudienceInteractionManager _AudienceInteractionManager;

        [SerializeField] private Text MessageFeed;

        // Start is called before the first frame update
        void Start()
        {
            InitializeAudienceEvents();
            InitializeItemSpawning();
            InitializeEndGame();

            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _AudienceInteractionManager.OnDisconnected += OnDisconnectedFromServer;
        }

        void Update()
        {
            UpdateEndGame();
        }

        void OnDisable()
        {
            _AudienceInteractionManager.OnDisconnected -= OnDisconnectedFromServer;
        }

        #region AudienceEvents

        public void OnDisconnectedFromServer()
        {
            StartCoroutine(QuitGame());
        }

        private IEnumerator QuitGame()
        {
            _AudienceInteractionManager?.ExitRoom(false);
            MessageFeed.text = "Disconnected from server. Game will quit in 5 seconds.";
            yield return new WaitForSeconds(5);
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
        }

        [Header("AudienceEvents")]
        EventManager m_audienceEventManager;
        public int m_maxEventVoteTime = 20, m_firstPollTime = 30, m_secondPollTime = 120;

        private void InitializeAudienceEvents()
        {
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_audienceEventManager = managers.GetComponentInChildren<EventManager>();
            StartCoroutine(LaunchPolls());
        }

        private int GetRandomEventIndex()
        {
            return Random.Range(0, (int)Events.EventID.max_id);
        }

        private IEnumerator LaunchPolls()
        {
            yield return new WaitForSeconds(m_firstPollTime);
            StartPoll();

            yield return new WaitForSeconds(m_secondPollTime);
            StartPoll();
        }

        private void StartPoll()
        {
            int eventA = GetRandomEventIndex();
            int eventB = GetRandomEventIndex();
            while (eventA == eventB)
            {
                eventB = GetRandomEventIndex();
            }

            m_audienceEventManager.StartPoll((Events.EventID)eventA, (Events.EventID)eventB, m_maxEventVoteTime);
        }

        #endregion


        #region ItemSpawning

        private ItemSpawner m_itemSpawner;

        private void InitializeItemSpawning()
        {
            m_itemSpawner = GetComponent<ItemSpawner>();
        }

        //example function
        private void ModulateSpawnRate(float timeChange)
        {
            m_itemSpawner.SpawnableItems["GHOST_PEPPER"].SpawnDelay += timeChange;
        }

        #endregion


        #region EndGame

        [Header("EndGame")]
        public Text m_winnerText;
        public Text m_rematchText, m_clock;
        private bool m_gameOver = false, m_acceptingInput = false;
        public static int REMATCH_TIMER = 10, GAME_TIMER = 240;
        [SerializeField] private int m_dominationDifference = 3;

        private void InitializeEndGame()
        {
            m_winnerText.enabled = false;
            m_rematchText.enabled = false;
            m_gameOver = false;
        }

        private void UpdateEndGame()
        {
            if (!m_gameOver)
            {
                int remainingTime = (int)(GAME_TIMER - Time.timeSinceLevelLoad);
                m_clock.text = FormatRemainingTime(remainingTime);
                if (remainingTime <= 0)
                {
                    GameOver();
                }
            }
        }

        private string FormatRemainingTime(int time)
        {
            int sec = time % 60;
            return time / 60 + ":" + (sec > 9 ? sec.ToString() : "0" + sec);
        }

        public void GameOver()
        {
            if (!m_gameOver)
            {
                m_gameOver = true;
                var winnerPlayer = DetermineWinner();
                m_winnerText.text = winnerPlayer.Name + " is the winner!";
                _AudienceInteractionManager?.ExitRoom(true, winnerPlayer.ID);                StartCoroutine(BackToMainMenuAfterShortPause());
            }
        }

        public PlayerManager DetermineWinner()
        {
            var players = Players.Dic;
            PlayerManager winner = null;
            var mostPotions = -1;

            for (int i = 0; i < players.Count; i++)
            {
                var numPotions = players[i].Score;
                if (numPotions > mostPotions)
                {
                    winner = players[i];
                    mostPotions = numPotions;
                }
            }

            return winner;
        }

        public void UpdateRanks()
        {
            var players = Players.Dic;
            List<PlayerManager> playerScores = new List<PlayerManager>();         
            for (int i = 0; i < players.Count; i++)
            {
                playerScores.Add(players[i]);
            }

            List<List<PlayerManager>> scoreGroups = playerScores.GroupBy(x => x.Score)
                                             .Select(x => x.ToList())
                                             .OrderByDescending(x => x[0].Score)
                                             .ToList();

            switch (scoreGroups.Count)
            {
                case 1:
                    for (int i = 0; i < players.Count; i++)     // When all players are even (e.g. at start), no one is in first
                    {
                        players[i].PlayerRank = PlayerManager.Rank.MIDDLE;
                    }
                    break;
                default:
                    RankGroup(scoreGroups[0], PlayerManager.Rank.FIRST);    
                    for (int i = 1; i < scoreGroups.Count; i++)
                    {
                        if (IsDominating(scoreGroups[0], scoreGroups[i]))
                            RankGroup(scoreGroups[i], PlayerManager.Rank.LAST);
                        else
                            RankGroup(scoreGroups[i], PlayerManager.Rank.MIDDLE);
                    }
                    break;
            }
        }

        private void RankGroup(List<PlayerManager> group, PlayerManager.Rank rank)
        {
            foreach (PlayerManager player in group)
            {
                player.PlayerRank = rank;
                Debug.Log("Player" + player + " is rank " + rank);
            }
        }

        private bool IsDominating(List<PlayerManager> group1, List<PlayerManager> group2)
        {
            return group1[0].Score - group2[0].Score >= m_dominationDifference;
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (m_acceptingInput)
            {
                if (input.GetActionID().Equals(con2.GamepadAction.ID.START)
                    || input.GetActionID().Equals(con2.GamepadAction.ID.INTERACT))
                {
                    SceneManager.LoadScene(SceneNames.Game);
                    m_gameOver = false;
                    m_acceptingInput = false;
                    return true;
                }
                else if (input.GetActionID().Equals(con2.GamepadAction.ID.PUNCH))
                {
                    SceneManager.LoadScene(SceneNames.MainMenu);
                    m_gameOver = false;
                    m_acceptingInput = false;
                    return true;
                }
            }
            return false;
        }

        private IEnumerator BackToMainMenuAfterShortPause()
        {
            yield return new WaitForSeconds(0.5f);
            m_winnerText.enabled = true;
            yield return new WaitForSeconds(0.5f);
            m_rematchText.enabled = true;
            for (int i = REMATCH_TIMER; i >= 0; i--)
            {
                m_rematchText.text = "Rematch?\n" + i;
                yield return new WaitForSeconds(1);
                if (i == 9)
                {
                    m_acceptingInput = true;
                }
            }

            m_gameOver = false;
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        #endregion
    }

}
