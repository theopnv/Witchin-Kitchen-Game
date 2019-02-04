using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PauseMenuController : NetworkBehaviour
{

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public Button resume, reset;
    private Text resetText;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown("escape"))   //start
        {
            if (isPaused)   //the player is trying to leave the pause menu
                Resume();
            else
                Pause();
            Debug.Log("Paused: " + isPaused);
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
        resume.Select();
        if (!isServer)
        {
            reset.interactable = false;
            resetText.text = "Ask host to reset";
        }
    }
}
