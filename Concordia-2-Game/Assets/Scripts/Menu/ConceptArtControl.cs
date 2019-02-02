using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConceptArtControl : MonoBehaviour {

    public GameObject ca1, ca2, ca3, leftArrow, rightArrow;
    public static bool changing;

    private void OnEnable()
    {
        changing = false;
    }

    // Update is called once per frame
    void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        if (!changing && horizontal < -0.9 || Input.GetKeyDown("left"))
        {
            changing = true;
            if (ca2.activeInHierarchy)       
                SwitchPics(ca2, ca1);
            else if (ca3.activeInHierarchy)
                SwitchPics(ca3, ca2);
            StartCoroutine(WaitOnChange());
        }
        else if (!changing && horizontal > 0.9 || Input.GetKeyDown("right"))
        {
            changing = true;
            if (ca1.activeInHierarchy)
                SwitchPics(ca1, ca2);
            else if (ca2.activeInHierarchy)
                SwitchPics(ca2, ca3);
            StartCoroutine(WaitOnChange());
        }

        if (ca1.activeInHierarchy && leftArrow.activeInHierarchy)
            leftArrow.SetActive(false);
        else if (ca3.activeInHierarchy && rightArrow.activeInHierarchy)
            rightArrow.SetActive(false);
        else if (ca2.activeInHierarchy)
        {
            if (!leftArrow.activeInHierarchy)
                leftArrow.SetActive(true);
            if (!rightArrow.activeInHierarchy)
                rightArrow.SetActive(true);
        }
    }

    private void SwitchPics(GameObject current, GameObject next)
    {
        current.SetActive(false);
        next.SetActive(true);
    }

    private IEnumerator WaitOnChange()
    {
        yield return new WaitForSeconds(0.3f);
        ConceptArtControl.changing = false;
    }
}
