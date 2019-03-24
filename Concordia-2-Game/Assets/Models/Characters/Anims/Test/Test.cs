using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    protected Animator anim;

    protected int LowerBodyLayerIdx;
    protected int UpperBodyLayerIdx;
    protected int SlapTrigger;
    protected int CarryTrigger;
    protected int DropTrigger;
    protected int SpellTrigger;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        LowerBodyLayerIdx = anim.GetLayerIndex("Lower body");
        UpperBodyLayerIdx = anim.GetLayerIndex("Upper body");
        SlapTrigger = Animator.StringToHash("Slap");
        CarryTrigger = Animator.StringToHash("Carry");
        DropTrigger = Animator.StringToHash("Drop");
        SpellTrigger = Animator.StringToHash("Spell");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger(SlapTrigger);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger(CarryTrigger);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger(DropTrigger);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger(SpellTrigger);
        }
    }
}
