using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectButton : MonoBehaviour, ISelectHandler
{
    public new AudioSource audio;

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        audio.Play();
    }
}
