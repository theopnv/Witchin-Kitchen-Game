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
        _AudienceInteractionManager.ExitRoom();

        SceneManager.LoadSceneAsync(SceneNames.MainMenu);
    }

    #endregion
}
