using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastwaveTrigger : MonoBehaviour
{
    public GameObject Target;

    [Range(0.0f, 10.0f)]
    public float RepeatWaitDuration = 1.0f;

    private Blastwave blastwave;

    // Start is called before the first frame update
    void Start()
    {
        blastwave = Target.GetComponent<Blastwave>();

        StartCoroutine(WaitThenPunch(RepeatWaitDuration));
    }

    IEnumerator WaitThenPunch(float time)
    {
        yield return new WaitForSeconds(time);
        blastwave.Play();

        StartCoroutine(WaitThenPunch(RepeatWaitDuration));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
