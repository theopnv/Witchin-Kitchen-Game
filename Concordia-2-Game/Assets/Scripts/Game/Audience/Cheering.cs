using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheering : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] cheers = new AudioClip[4];

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Cheer(int playerId)
    {
        audioSource.clip = cheers[playerId];
        StartCoroutine(Cheer());
    }

    private IEnumerator Cheer()
    {
        audioSource.Play();
        yield return new WaitForSeconds(2);
        audioSource.Play();
        yield return new WaitForSeconds(2);
        audioSource.Play();
    }

}
