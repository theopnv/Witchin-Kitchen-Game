using System.Collections;
using System.Collections.Generic;
using con2;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{

    #region Private Variables

    private static bool IsPaused;

    [SerializeField]
    private Canvas PauseMenuUIInstance;

    private AudienceInteractionManager _AudienceInteractionManager;

    #endregion

    #region Unity API

    void Start()
    {
        _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (IsPaused)   //the player is trying to leave the pause menu
                Resume();
            else
                Pause();
            Debug.Log("Paused: " + IsPaused);
        }
    }

    #endregion

    #region Custom Methods

    public void Resume()
    {
        IsPaused = false;
        PauseMenuUIInstance.gameObject.SetActive(IsPaused);
    }

    public void Pause()
    {
        IsPaused = true;
        PauseMenuUIInstance.gameObject.SetActive(IsPaused);
    }

    public void OnResumeClick()
    {
        Resume();
    }

    public void OnExitClick()
    {
        // Warn all viewers that the game is about to be finished
        _AudienceInteractionManager.ExitRoom();

        // Destroy the DontDestroyOnLoad objects because they will be re-created
        // again in the main menu and lobby.
        Destroy(_AudienceInteractionManager.gameObject);
        Destroy(FindObjectOfType<DevModeCommands>().gameObject);

        SceneManager.LoadSceneAsync(SceneNames.MainMenu);
    }

    #endregion
}
