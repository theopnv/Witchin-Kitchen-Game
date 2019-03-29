using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{

    public class MainGameManager : AMainManager
    {
        [SerializeField] private GameObject _LoadingPanel;
        private const float LOADING_TIME = 4f;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            Debug.Log("Starting game with " + GameInfo.PlayerNumber + " players");
            for (var i = 0; i < GameInfo.PlayerNumber; i++)
            {
                ActivatePlayer(true, i);
            }

            InitializeAudienceEvents();
            InitializeEndGame();

            _AudienceInteractionManager.OnDisconnected += OnDisconnectedFromServer;

            // Comment if you want to start the game scene right from the start
            StartCoroutine(ExitLoadingScreen());
            // End
        }

        protected override void Update()
        {
            base.Update();
            if (!m_countdown)
            {
                UpdateEndGame();
            }
        }

        void OnDisable()
        {
            _AudienceInteractionManager.OnDisconnected -= OnDisconnectedFromServer;
        }

        private IEnumerator ExitLoadingScreen()
        {
            AudioListener.volume = 0.0f;

            for (var i = 0; i < GameInfo.PlayerNumber; i++)
            {
                GamepadMgr.Pad(i).BlockGamepad(true);
            }
            _LoadingPanel.SetActive(true);
            _LoadingPanel.GetComponent<LoadingScreenManager>().Title.text = "Loading...";

            yield return new WaitForSeconds(LOADING_TIME);
            AudioListener.volume = 1f;
            
            StartCoroutine(MiniTransition.Get().SequenceOut(null, _HidePanel()));
            StartCoroutine(StartGame());
        }

        private IEnumerator _HidePanel()
        {
            _LoadingPanel.SetActive(false);
            yield return null;
        }

        public override List<IInputConsumer> GetInputConsumers(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();

            // Misc.
            var pmi = GetComponent<PauseMenuInstantiator>();
            inputConsumers.Add(pmi);

            // Players
            var player = PlayersInstances[playerIndex];
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
            m_audioSource.clip = voteCue;
            m_audioSource.Play();
        }

        #endregion

        #region EndGame

        [Header("GameManagement")]
        public Text m_clock;
        public Text m_gameOverText;
        public static bool GAME_OVER = false;
        private bool m_acceptingInput = false;
        public float GAME_TIMER = 240;
        private bool m_countdown = false;
        private const int COUNTDOWN_TIME = 3;
        private int m_dominationDifference = 3;

        private EndGameManager EGM;
        public GameObject borders, title;
        private int m_currentLeaderId = -1;

        private AudioSource m_audioSource;
        public AudioClip ClockTick, voteCue;
        private Cheering cheers;

        private IEnumerator StartGame()
        {
            m_audioSource = GetComponent<AudioSource>();
            if (m_audioSource)
            {
                m_audioSource.PlayDelayed(0.1f); //Start trumpet
            }

            m_countdown = true;
            var fontSize = m_clock.fontSize;
            m_clock.fontSize = 200;
            m_clock.text = "3";
            yield return new WaitForSeconds(1);
            m_clock.text = "2";
            yield return new WaitForSeconds(1);
            m_clock.text = "1";
            yield return new WaitForSeconds(1);

            m_clock.fontSize = fontSize;
            for (var i = 0; i < GameInfo.PlayerNumber; i++)
            {
                GamepadMgr.Pad(i).BlockGamepad(false);
            }

            var gameMusic = GameObject.Find("ArenaMusic").GetComponent<AudioSource>();
            gameMusic.Play();

            m_countdown = false;

            cheers = GetComponent<Cheering>();
            StartCoroutine(PrepareCheers());
        }

        private void InitializeEndGame()
        {
            EGM = FindObjectOfType<EndGameManager>();
            EGM.AudienceInteractionManager = _AudienceInteractionManager;
            m_gameOverText.enabled = false;
            GAME_OVER = false;
        }

        private bool largeClockStarted = false;
        private void UpdateEndGame()
        {
            if (!GAME_OVER)
            {
                int remainingTime = (int)(GAME_TIMER + LOADING_TIME + COUNTDOWN_TIME - Time.timeSinceLevelLoad);
                m_clock.text = FormatRemainingTime(remainingTime);
                if (remainingTime == 10 && !largeClockStarted)
                {
                    largeClockStarted = true;
                    m_clock.fontSize = 200;
                    StartCoroutine(TickingSounds());
                }
                if (remainingTime <= 0)
                {
                    GameOver();
                }
            }
        }

        private IEnumerator TickingSounds()
        {
            m_audioSource.clip = ClockTick;
            if (m_audioSource)
            {
                int count = 0;
                while (count < 10)
                {
                    count++;
                    m_audioSource.Play();
                    yield return new WaitForSeconds(1.0f);
                }
            }
        }

        private IEnumerator PrepareCheers()
        {
            //Cheering at occasional intervals
            yield return new WaitForSeconds(22);
            cheers.Cheer(GetLeaderId());
            yield return new WaitForSeconds(42);
            cheers.Cheer(GetLeaderId());
            yield return new WaitForSeconds(41);
            cheers.Cheer(GetLeaderId());
            yield return new WaitForSeconds(57);
            cheers.Cheer(GetLeaderId());
            yield return new WaitForSeconds(39);
            cheers.Cheer(GetLeaderId());
        }

        private int GetLeaderId()
        {
            var finalRankings = new List<List<PlayerManager>>();
            var playerScores = new List<PlayerManager>();
            for (int i = 0; i < PlayersInstances.Count; i++)
            {
                playerScores.Add(GetPlayerById(i));
            }

            List<List<PlayerManager>> rankings = playerScores.GroupBy(x => x.CompletedPotionCount)
                                             .Select(x => x.ToList())
                                             .OrderByDescending(x => x[0].CompletedPotionCount)
                                             .ToList();

            List<List<PlayerManager>> tieBreaker = rankings[0].GroupBy(x => x.CollectedIngredientCount)
                                .Select(x => x.ToList())
                                .OrderByDescending(x => x[0].CollectedIngredientCount)
                                .ToList();
            if (tieBreaker[0].Count > 1)
                return -1;
            return tieBreaker[0][0].ID;
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
            if (!GAME_OVER)
            {
                GAME_OVER = true;
                m_gameOverText.enabled = true;
                m_clock.enabled = false;

                DetermineWinner();
                _AudienceInteractionManager?.SendGameOutcome();

                StartCoroutine(Transition.Get().SequenceIn(ShowLeaderboardWait(), ShowLeaderboard()));
            }
        }

        private IEnumerator ShowLeaderboardWait()
        {
            yield return new WaitForSeconds(3.0f);
        }

        private IEnumerator ShowLeaderboard()
        {
            SceneManager.LoadScene(SceneNames.Leaderboard);
            yield return null;
        }

        public void DetermineWinner()
        {
            var finalRankings = new List<List<PlayerManager>>();
            var playerScores = new List<PlayerManager>();
            for (int i = 0; i < PlayersInstances.Count; i++)
            {
                playerScores.Add(GetPlayerById(i));
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
                    finalRankings.Add(scoreGroup);
                }
            }

            EGM.SetFinalRankings(finalRankings);
        }

        private void SetBorderColor()
        {
            var newLeaderId = GetLeaderId();
            if (newLeaderId != m_currentLeaderId)
            {
                m_currentLeaderId = newLeaderId;
                borders.GetComponentInChildren<MeshRenderer>().material = ColorsManager.Get().PlayerBorderMaterials[newLeaderId];
                title.GetComponentInChildren<MeshRenderer>().material = ColorsManager.Get().PlayerBorderMaterials[newLeaderId];
            }
        }

        public void UpdateRanks()
        {
            List<PlayerManager> playerScores = new List<PlayerManager>();
            for (int i = 0; i < PlayersInstances.Count; i++)
            {
                playerScores.Add(GetPlayerById(i));
            }

            List<List<PlayerManager>> scoreGroups = playerScores.GroupBy(x => x.CompletedPotionCount)
                                             .Select(x => x.ToList())
                                             .OrderByDescending(x => x[0].CompletedPotionCount)
                                             .ToList();

            switch (scoreGroups.Count)
            {
                case 1:
                    for (int i = 0; i < PlayersInstances.Count; i++)     // When all players are even (e.g. at start), no one is in first
                    {
                        GetPlayerById(i).PlayerRank = PlayerManager.Rank.MIDDLE;
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
            SetBorderColor();
        }

        private void RankGroup(List<PlayerManager> group, PlayerManager.Rank rank)
        {
            foreach (PlayerManager player in group)
            {
                player.PlayerRank = rank;
            }
        }

        private bool IsDominating(List<PlayerManager> group1, List<PlayerManager> group2)
        {
            return group1[0].CompletedPotionCount - group2[0].CompletedPotionCount >= m_dominationDifference;
        }





        #endregion
    }

}
