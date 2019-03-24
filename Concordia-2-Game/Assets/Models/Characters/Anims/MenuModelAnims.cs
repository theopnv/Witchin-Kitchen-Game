using UnityEngine;
using System.Collections;

public class MenuModelAnims : MonoBehaviour
{
    protected Animator Anim;

    protected int CarryTrigger;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();

        CarryTrigger = Animator.StringToHash("Carry");
    }

    public void Carry()
    {
        SetTrigger(CarryTrigger);
    }

    protected void SetTrigger(int trigger)
    {
        Anim.SetTrigger(trigger);
    }

}
