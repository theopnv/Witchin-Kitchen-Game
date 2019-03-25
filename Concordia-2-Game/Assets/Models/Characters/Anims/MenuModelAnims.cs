using UnityEngine;
using System.Collections;

public class MenuModelAnims : MonoBehaviour
{
    protected Animator Anim;

    protected int CarryTrigger;

    public void Carry()
    {
        Anim = GetComponent<Animator>();
        CarryTrigger = Animator.StringToHash("Carry");
        SetTrigger(CarryTrigger);
    }

    protected void SetTrigger(int trigger)
    {
        Anim.SetTrigger(trigger);
    }

}
