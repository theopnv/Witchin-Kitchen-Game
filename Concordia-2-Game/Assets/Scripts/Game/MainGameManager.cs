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
        m_itemSpawner = GetComponent<ItemSpawner>();

        m_winnerText.enabled = false;
        m_rematchText.enabled = false;
        m_gameOver = false;
    }

    #region AudienceEvents

    #endregion


    #region ItemSpawning
    private ItemSpawner m_itemSpawner;

    //example function
    private void ModulateSpawnRate(float timeChange)
    {
        m_itemSpawner.SpawnableItems["GHOST_PEPPER"].SpawnDelay += timeChange;
    }

    #endregion


    #region EndGame

    public Text m_winnerText, m_rematchText;
    private bool m_gameOver = false;

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
        if (m_gameOver)
        {
            if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.START)
                || input.GetActionID().Equals(con2.GamepadAction.ButtonID.INTERACT))
            {
                SceneManager.LoadScene(SceneNames.Game);
                m_gameOver = false;
                return true;
            }
            else if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.PUNCH))
            {
                SceneManager.LoadScene(SceneNames.MainMenu);
                m_gameOver = false;
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
        }

        m_gameOver = false;
        SceneManager.LoadScene(SceneNames.MainMenu);
    }

    #endregion
}
