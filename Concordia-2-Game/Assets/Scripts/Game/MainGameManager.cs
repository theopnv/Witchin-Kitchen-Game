using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        m_winnerText.enabled = false;
        m_itemSpawner = GetComponent<ItemSpawner>();
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

    public Text m_winnerText;
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

    private IEnumerator BackToMainMenuAfterShortPause()
    {
        yield return new WaitForSeconds(1);
        m_winnerText.enabled = true;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneNames.MainMenu);
    }

    #endregion
}
