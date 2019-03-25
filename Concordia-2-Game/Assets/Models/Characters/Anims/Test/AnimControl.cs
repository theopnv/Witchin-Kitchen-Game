using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl : MonoBehaviour
{
    protected Animator Anim;
    protected bool Active;

    protected int SlapTrigger;
    protected int CarryTrigger;
    protected int DropTrigger;
    protected int SpellTrigger;
    protected int RunningBool;
    protected int RunSpeedFloat;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();

        Active = Anim != null;

        SlapTrigger = Animator.StringToHash("Slap");
        CarryTrigger = Animator.StringToHash("Carry");
        DropTrigger = Animator.StringToHash("Drop");
        SpellTrigger = Animator.StringToHash("Spell");
        RunningBool = Animator.StringToHash("Running");
        RunSpeedFloat = Animator.StringToHash("RunSpeed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Slap()
    {
        SetTrigger(SlapTrigger);
    }

    public void Carry()
    {
        SetTrigger(CarryTrigger);
    }

    public void Drop()
    {
        SetTrigger(DropTrigger);
    }

    public void Spell()
    {
        SetTrigger(SpellTrigger);
    }

    public void SetRunning(bool running)
    {
        SetBool(RunningBool, running);
    }

    public void SetRunSpeed(float runSpeed)
    {
        SetFloat(RunSpeedFloat, runSpeed);
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
