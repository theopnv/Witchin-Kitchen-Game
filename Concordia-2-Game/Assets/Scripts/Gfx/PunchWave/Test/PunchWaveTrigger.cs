using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchWaveTrigger : MonoBehaviour
{
    public GameObject Target;

    [Range(0.0f, 10.0f)]
    public float RepeatWaitDuration = 1.0f;

    private PunchWave punchWave;

    // Start is called before the first frame update
    void Start()
    {
        punchWave = Target.GetComponent<PunchWave>();

        StartCoroutine(WaitThenPunch(RepeatWaitDuration));
    }

    IEnumerator WaitThenPunch(float time)
    {
        yield return new WaitForSeconds(time);
        punchWave.Play();

        StartCoroutine(WaitThenPunch(RepeatWaitDuration));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
