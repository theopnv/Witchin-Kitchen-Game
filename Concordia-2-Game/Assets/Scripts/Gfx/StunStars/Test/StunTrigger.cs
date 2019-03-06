using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTrigger : MonoBehaviour
{
    public GameObject Target;

    [Range(0.0f, 10.0f)]
    public float Duration = 2.0f;

    private StunStars stars;

    // Start is called before the first frame update
    void Start()
    {
        stars = Target.GetComponent<StunStars>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stars.IsPlaying())
        {
            stars.Play(Duration);
        }
    }
}
