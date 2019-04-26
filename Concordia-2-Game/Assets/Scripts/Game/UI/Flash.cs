using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    private Image thingToFlash;
    public float flashFrequency = 0.75f;

    void Start()
    {
        thingToFlash = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(FlashParty());
    }

    private IEnumerator FlashParty()
    {
        bool tempEnabled = false;
        while (true)
        {
            yield return new WaitForSeconds(flashFrequency);
            thingToFlash.enabled = tempEnabled;
            tempEnabled = !tempEnabled;
        }
    }
}
