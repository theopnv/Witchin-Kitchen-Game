using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{

    public class MainGameManager : AMainManager, IInputConsumer
    {
        private AudienceInteractionManager _AudienceInteractionManager;

        private MessageFeedManager _MessageFeedManager;

        void Awake()
        {
            if (Application.isEditor && PlayersInfo.PlayerNumber == 0)
            {
                PlayersInfo.PlayerNumber = 4;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Initialize players
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                FindObjectOfType<SpawnPlayersControllerGame>().InstantiatePlayer(i, OnPlayerInitialized);
            }
            InitializeAudienceEvents();
            InitializeItemSpawning();
            InitializeEndGame();

            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _AudienceInteractionManager.OnDisconnected += OnDisconnectedFromServer;

            _MessageFeedManager = FindObjectOfType<MessageFeedManager>();
        }

        void Update()
        {
            base.Update();
            UpdateEndGame();
        }

        void OnDisable()
        {
            _AudienceInteractionManager.OnDisconnected -= OnDisconnectedFromServer;
        }

        public override List<IInputConsumer> GetInputConsumers(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();

            // Misc.
            inputConsumers.Add(this);
            var pmi = GetComponent<PauseMenuInstantiator>();
            inputConsumers.Add(pmi);

            // Players
            var player = Players[playerIndex];
            inputConsumers.Add(player.GetComponent<FightStun>());

            // Kitchens
            var kitchenParents = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
            var kitchenStations = new List<ACookingMinigame>();
            foreach (var kitchen in kitchenParents)
            {
                var stations = kitchen.GetComponentsInChildren<ACookingMinigame>();
                kitchenStations.AddRange(stations);
            }
            foreach (ACookingMinigame station in kitchenStations)
            {
                inputConsumers.Add(station);
            }

            inputConsumers.Add(player.GetComponent<PlayerInputController>());
            return inputConsumers;
        }

        #region AudienceEvents

        public void OnDisconnectedFromServer()
        {
            StartCoroutine(QuitGame());
        }

        private IEnumerator QuitGame()
        {
            _AudienceInteractionManager.SendEndGame(false);
            const string msg = "Disconnected from server. Game will quit in 5 seconds.";
            _MessageFeedManager.AddMessageToFeed(msg, MessageFeedManager.MessageType.error);
            yield return new WaitForSeconds(5);
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
        }

        [Header("AudienceEvents")]
        EventManager m_audienceEventManager;
        public int 
            m_maxEventVoteTime = 20, 
            m_firstPollTime = 30, 
            m_poll_frequency = 60, 
            m_max_poll_number = 3, 
            m_poll_number = 0;

        private void InitializeAudienceEvents()
        {
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            m_audienceEventManager = managers.GetComponentInChildren<EventManager>();
            InvokeRepeating("LaunchPoll", m_firstPollTime, m_poll_frequency);
        }

        private int GetRandomEventIndex()
        {
            return Random.Range(0, (int)Events.EventID.max_id);
        }

        private void LaunchPoll()
        {
            if (m_poll_number >= m_max_poll_number)
            {
                return;
            }
            StartPoll();
            ++m_poll_number;
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
            //m_itemSpawner.SpawnableItems[Ingredient.PEPPER].SpawnDelay += timeChange;
        }

        #endregion

        #region EndGame

        [Header("EndGame")]
        public Text m_winnerText;
        public Text m_gameOverText, m_rematchText, m_clock;
        public GameObject m_backdrop;
        private List<List<PlayerManager>> m_finalRankings;
        private bool m_gameOver = false, m_acceptingInput = false;
        public int REMATCH_TIMER = 10, GAME_TIMER = 240;
        [SerializeField] private int m_dominationDifference = 3;

        private void InitializeEndGame()
        {
            m_backdrop.SetActive(false);
            m_gameOverText.enabled = false;
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
                if (remainingTime == 10)
                {
                    m_clock.fontSize = 200;
                }
                if (remainingTime <= 0)
                {
                    GameOver();
                }
            }
        }

        private string FormatRemainingTime(int time)
        {
            int sec = time % 60;
            var timeString = "";
            if (time > 10)
                timeString = time / 60 + ":" + (sec > 9 ? sec.ToString() : "0" + sec);
            else
                timeString = time.ToString();

            return timeString;
        }

        public void GameOver()
        {
            if (!m_gameOver)
            {
                m_gameOver = true;
                m_gameOverText.enabled = true;
                m_clock.enabled = false;

                DetermineWinner();
                StartCoroutine(ShowLeaderboard());
               }
        }

        private IEnumerator ShowLeaderboard()
        {
            yield return new WaitForSeconds(2.0f);
            m_backdrop.SetActive(true);
            m_winnerText.text = "";
            foreach (var scoregroup in m_finalRankings)
            {
                foreach (var player in scoregroup)
                {
                    var count = player.CollectedIngredientCount;
                    m_winnerText.text += player.Name + " collected " + count + " ingredient" + (count == 1 ? "" : "s") + "\n\n";
                }
            }
            _AudienceInteractionManager?.SendGameOutcome();
            StartCoroutine(BackToMainMenuAfterShortPause());
        }

        public void DetermineWinner()
        {
            m_finalRankings = new List<List<PlayerManager>>();
            List<PlayerManager> playerScores = new List<PlayerManager>();
            for (int i = 0; i < Players.Count; i++)
            {
                playerScores.Add(Players[i]);
            }

            List<List<PlayerManager>> rankings = playerScores.GroupBy(x => x.CompletedPotionCount)
                                             .Select(x => x.ToList())
                                             .OrderByDescending(x => x[0].CompletedPotionCount)
                                             .ToList();

            for (int i = 0; i < rankings.Count; i++)
            {
                List<List<PlayerManager>> tieBreaker = rankings[i].GroupBy(x => x.CollectedIngredientCount)
                                 .Select(x => x.ToList())
                                 .OrderByDescending(x => x[0].CollectedIngredientCount)
                                 .ToList();
                foreach (var scoreGroup in tieBreaker)
                {
                    m_finalRankings.Add(scoreGroup);
                }
            }
        }

        public void UpdateRanks()
        {
            List<PlayerManager> playerScores = new List<PlayerManager>();         
            for (int i = 0; i < Players.Count; i++)
            {
                playerScores.Add(Players[i]);
            }

            List<List<PlayerManager>> scoreGroups = playerScores.GroupBy(x => x.CompletedPotionCount)
                                             .Select(x => x.ToList())
                                             .OrderByDescending(x => x[0].CompletedPotionCount)
                                             .ToList();

            switch (scoreGroups.Count)
            {
                case 1:
                    for (int i = 0; i < Players.Count; i++)     // When all players are even (e.g. at start), no one is in first
                    {
                        Players[i].PlayerRank = PlayerManager.Rank.MIDDLE;
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
            return group1[0].CompletedPotionCount - group2[0].CompletedPotionCount >= m_dominationDifference;
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (m_acceptingInput)
            {
                if (input.GetActionID().Equals(con2.GamepadAction.ID.START)
                    || input.GetActionID().Equals(con2.GamepadAction.ID.INTERACT))
                {
                    _AudienceInteractionManager.SendEndGame(true);
                    SceneManager.LoadScene(SceneNames.Game);
                    m_acceptingInput = false;
                    m_winnerText.text = "";
                    m_rematchText.enabled = false;
                    return true;
                }

                if (input.GetActionID().Equals(con2.GamepadAction.ID.PUNCH))
                {
                    SceneManager.LoadScene(SceneNames.MainMenu);
                    m_acceptingInput = false;
                    m_winnerText.enabled = false;
                    m_rematchText.enabled = false;
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
                if (i == REMATCH_TIMER - 1)
                {
                    m_acceptingInput = true;
                }
            }

            m_gameOver = false;
            _AudienceInteractionManager.SendEndGame(false);
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        #endregion
    }

}
