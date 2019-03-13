using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTrigger : MonoBehaviour
{
    public GameObject Target;

    [Range(0.0f, 10.0f)]
    public float RepeatWaitDuration = 2.0f;

    private Freeze freeze;

    // Start is called before the first frame update
    void Start()
    {
        freeze = Target.GetComponent<Freeze>();

        StartCoroutine(WaitThenFreeze(RepeatWaitDuration));
    }

    IEnumerator WaitThenFreeze(float time)
    {
        yield return new WaitForSeconds(time);
        freeze.PlayFreeze();

        StartCoroutine(WaitThenThaw(RepeatWaitDuration));
    }

    IEnumerator WaitThenThaw(float time)
    {
        yield return new WaitForSeconds(time);
        freeze.PlayThaw();

        StartCoroutine(WaitThenFreeze(RepeatWaitDuration));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
