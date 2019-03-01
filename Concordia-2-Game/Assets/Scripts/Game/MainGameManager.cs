using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour, IInputConsumer
{
    private AudienceInteractionManager _AudienceInteractionManager;

    // Start is called before the first frame update
    void Start()
    {
        InitializeAudienceEvents();
        InitializeItemSpawning();
        InitializeEndGame();

        _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
    }


    void Update()
    {
        UpdateEndGame();
    }


    #region AudienceEvents

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
            GameObject winnerPlayer = DetermineWinner();
            m_winnerText.text = winnerPlayer.name + " is the winner!";
            _AudienceInteractionManager?.ExitRoom(true, 1); // TODO: HARDCODED: WINNER INDEX MUST ABSOLUTELY BE DYNAMIC
            StartCoroutine(BackToMainMenuAfterShortPause());
        }
    }

    private GameObject DetermineWinner()
    {
        GameObject[] cauldrons = GameObject.FindGameObjectsWithTag(Tags.CAULDRON);
        GameObject winner = null;
        int mostPotions = -1;

        foreach (GameObject cauldron in cauldrons)
        {
            RecipeManager manager = cauldron.GetComponent<RecipeManager>();
            int numPotions = manager.GetNumCompletedPotions();
            if (numPotions > mostPotions)
            {
                winner = cauldron.GetComponent<ACookingMinigame>().GetStationOwner();
                mostPotions = numPotions;
            }
        }

        return winner;
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (m_acceptingInput)
        {
            if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.START)
                || input.GetActionID().Equals(con2.GamepadAction.ButtonID.INTERACT))
            {
                SceneManager.LoadScene(SceneNames.Game);
                m_gameOver = false;
                m_acceptingInput = false;
                return true;
            }
            else if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.PUNCH))
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
