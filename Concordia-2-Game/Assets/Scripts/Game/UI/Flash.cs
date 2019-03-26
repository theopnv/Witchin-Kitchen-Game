using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    private Image thingToFlash;

    void Start()
    {
        thingToFlash = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == con2.SceneNames.Lobby)
        {
            StartCoroutine(FlashParty());
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashParty()
    {
        bool tempEnabled = false;
        while (true)
        {
            yield return new WaitForSeconds(0.75f);
            thingToFlash.enabled = tempEnabled;
            tempEnabled = !tempEnabled;
        }
    }
}
