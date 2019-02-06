using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFightState : MonoBehaviour
{
    // Public

    public static GlobalFightState get()
    {
        return instance;
    }

    public void AddFighter(Fighter fighter)
    {
        fighters.Add(fighter.gameObject);
    }

    public float PunchUpwardsForce = 1.0f;
    public float PunchForceMultiplier = 5.0f;

    public List<FighterPunch.PunchEvent> Punches = new List<FighterPunch.PunchEvent>();


    // Private

    private static GlobalFightState instance;

    private List<GameObject> fighters = new List<GameObject>();


    // Events

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach (var punch in Punches)
        {
            var knockDirection = punch.TargetPosition - punch.SourcePosition;
            knockDirection.y = PunchUpwardsForce;
            knockDirection.Normalize();
            knockDirection *= PunchForceMultiplier;

            var targetBody = punch.Target.GetComponent<Rigidbody>();
            targetBody.AddForce(knockDirection, ForceMode.Impulse);
        }

        // Setup for next frame
        Punches.Clear();
    }
}
