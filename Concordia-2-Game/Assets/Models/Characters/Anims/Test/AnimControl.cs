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

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();

        Active = Anim != null;

        SlapTrigger = Animator.StringToHash("Slap");
        CarryTrigger = Animator.StringToHash("Carry");
        DropTrigger = Animator.StringToHash("Drop");
        SpellTrigger = Animator.StringToHash("Spell");
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

    protected void SetTrigger(int trigger)
    {
        Anim.SetTrigger(trigger);
    }
}
