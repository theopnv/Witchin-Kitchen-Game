using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    protected static Pulse Instance;

    public static void Pulsate()
    {
        if (Instance != null)
        {
            Instance.Play();
        }
    }


    SpriteRenderer Rend;

    public bool Debug = false;

    public float Duration = 3.0f;
    public float Frequency = 20.0f;
    public float Intensity = 0.5f;

    public AnimationCurve YAnim;
    public float YDisplacement;

    protected bool Playing = false;
    protected float StartTime;
    protected Color BaseColor;
    protected float BaseY;
    
    void Awake()
    {
        if (Instance == null && gameObject.activeInHierarchy)
        {
            Instance = this;

            if (Debug)
                print("Set pulse isntance to | " + gameObject + " | " + Time.frameCount);
        }

        Rend = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void Play()
    {
        Playing = true;
        StartTime = Time.time;
        BaseColor = Rend.color;
        BaseY = transform.position.y;

        if (Debug)
            print("Play pulse | " + BaseY + " | " + BaseColor + " | " + Time.frameCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug && Input.GetKeyDown(KeyCode.O))
            Play();

        if (Playing)
        {
            var elapsed = Time.time - StartTime;
            var progress = Mathf.Clamp01(elapsed / Duration);
            if (progress >= 1.0f)
            {
                Playing = false;
                Rend.color = BaseColor;
                var p = transform.position;
                p.y = BaseY;
                transform.position = p;
            }
            else
            {
                float func = Mathf.Sin(progress * progress * Frequency * Mathf.PI - Mathf.PI * 0.5f) / 2.0f + 0.5f;
                var emissive = 1.0f + Intensity * func;

                Rend.color = BaseColor * emissive;

                var pos = transform.position;
                pos.y = BaseY + YAnim.Evaluate(progress) * YDisplacement;
                transform.position = pos;
            }
        }
    }
}
