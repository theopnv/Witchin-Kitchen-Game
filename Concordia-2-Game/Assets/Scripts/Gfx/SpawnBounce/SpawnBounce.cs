﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBounce : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Playback = 0.0f;

    public float AnimTime = 1.0f;

    public AnimationCurve Scale;

    [Range(0.0f, 40.0f)]
    public float Impulse = 1.0f;

    [Range(0.0f, 40.0f)]
    public float TempGravity = 0.0f;

    public GameObject ScaleObject;
    public Rigidbody OwnBody;

    public bool AutoPlay = false;

    private float StartTime;
    private bool Playing = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (AutoPlay)
        {
            Play();
        }
    }

    public void Setup(GameObject scaleObject, GameObject physicsObject)
    {
        ScaleObject = scaleObject;
        OwnBody = physicsObject.GetComponent<Rigidbody>();
    }

    public void Play()
    {
        OwnBody.freezeRotation = true;
        OwnBody.AddForce(Vector3.up * Impulse, ForceMode.VelocityChange);

        StartTime = Time.time;
        Playing = true;
    }

    // Won't cancel the upwards impulse but that's ok since
    // this is called only for the gift spell where the player
    // holding the item cancels the impulse.
    // FreezeRotation isn't cancelled since it should be true while the ingredient is held.
    public void CancelForGift()
    {
        Playing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Playing)
            return;

        // Playback
        var curTime = Time.time;
        var elapsed = curTime - StartTime;

        if (elapsed > AnimTime) // Done animation
        {
            OwnBody.freezeRotation = false;

            Playing = false;
        }

        Playback = elapsed / AnimTime;
        Playback = Mathf.Clamp01(Playback);


        OwnBody.AddForce(Vector3.down * TempGravity, ForceMode.Acceleration);

        var scale = Scale.Evaluate(Playback);
        ScaleObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}
