using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PauseMenuController : NetworkBehaviour
{

    #region Public Variables

    public static bool IsPaused;
    public GameObject PauseMenuUIPrefab;
    private Text ResetText;

    #endregion

    #region Unity API

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown("escape"))   //start
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
        PauseMenuUIPrefab.SetActive(false);
    }

    public void Pause()
    {
        IsPaused = true;
        PauseMenuUIPrefab.SetActive(true);
    }

    public void OnResumeClick()
    {
        Resume();
    }

    public void OnExitClick()
    {
        Debug.Log("EXITING");
    }

    #endregion
}
