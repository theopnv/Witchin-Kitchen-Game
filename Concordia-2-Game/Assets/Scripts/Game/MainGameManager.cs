using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour, IInputConsumer
{
    public static int REMATCH_TIMER = 10;

    // Start is called before the first frame update
    void Start()
    {
        InitializeAudienceEvents();
        InitializeItemSpawning();
        InitializeEndGame();
    }

    #region AudienceEvents

    [Header("AudienceEvents")]
    EventManager m_audienceEventManager;
    public Text m_audienceEventText;
    public int m_maxEventVoteTime = 20, m_firstPollTime = 30, m_secondPollTime = 120;

    private void InitializeAudienceEvents()
    {
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        m_audienceEventManager = managers.GetComponentInChildren<EventManager>();
        m_audienceEventText.enabled = false;
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
        m_audienceEventText.text = "Time for an audience event, spectators vote on your phone!";
        m_audienceEventText.enabled = true;

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
    public Text m_rematchText;
    private bool m_gameOver = false, m_acceptingInput = false;

    private void InitializeEndGame()
    {
        m_winnerText.enabled = false;
        m_rematchText.enabled = false;
        m_gameOver = false;
    }

    //For MVP, first person to complete a potion wins. Will require serious reworking when win is time&point based
    public void Gameover(GameObject winnerPlayer)
    {
        if (!m_gameOver)
        {
            m_gameOver = true;
            m_winnerText.text = winnerPlayer.name + " is the winner!";
            StartCoroutine(BackToMainMenuAfterShortPause());
        }
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
