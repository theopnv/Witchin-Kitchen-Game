using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControlNewt : MonoBehaviour
{
    protected Animator Anim;
    protected bool Active;

    protected int WalkingBool;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();

        Active = Anim != null;
        
        WalkingBool = Animator.StringToHash("Walking");
    }

    public void SetWalking(bool running)
    {
        if (Anim?.GetBool(WalkingBool) != running)
            SetBool(WalkingBool, running);
    }


    protected void SetTrigger(int trigger)
    {
        if (Active)
            Anim.SetTrigger(trigger);
    }

    protected void SetBool(int param, bool value)
    {
        if (Active)
            Anim.SetBool(param, value);
    }

    protected void SetFloat(int param, float value)
    {
        if (Active)
            Anim.SetFloat(param, value);
    }
}
