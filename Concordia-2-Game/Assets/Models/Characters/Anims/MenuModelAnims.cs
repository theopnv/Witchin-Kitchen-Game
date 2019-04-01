using UnityEngine;
using System.Collections;

public class MenuModelAnims : MonoBehaviour
{
    protected Animator Anim;

    protected int CarryTrigger;
    protected int DizzyBool;
    protected int CarryingBool;

    public void Carry()
    {
        Anim = GetComponent<Animator>();
        CarryTrigger = Animator.StringToHash("Carry");
        CarryingBool = Animator.StringToHash("Carrying");
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
        Anim.SetBool(CarryingBool, true);
    }

    protected void SetBool(int param, bool value)
    {
        Anim.SetBool(param, value);
    }
}
