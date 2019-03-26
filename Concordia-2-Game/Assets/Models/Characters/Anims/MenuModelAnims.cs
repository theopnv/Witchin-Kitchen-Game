using UnityEngine;
using System.Collections;

public class MenuModelAnims : MonoBehaviour
{
    protected Animator Anim;

    protected int CarryTrigger;
    protected int DizzyBool;

    public void Carry()
    {
        Anim = GetComponent<Animator>();
        CarryTrigger = Animator.StringToHash("Carry");
        SetTrigger(CarryTrigger);
    }

    public void Dizzy()
    {
        Anim = GetComponent<Animator>();
        DizzyBool = Animator.StringToHash("Dizzy");
        SetBool(DizzyBool, true);
    }

    protected void SetTrigger(int trigger)
    {
        Anim.SetTrigger(trigger);
    }

    protected void SetBool(int param, bool value)
    {
        Anim.SetBool(param, value);
    }
}
