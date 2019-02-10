using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFightState : MonoBehaviour
{
    // Public

    public static string PLAYER_CAPSULE_TAG = "PlayerCapsule";

    public static GlobalFightState get()
    {
        return instance;
    }

    public void AddFighter(GameObject fighter)
    {
        fighters.Add(fighter);
    }

    public float PunchUpwardsForce = 1.0f;
    public float PunchForceMultiplier = 5.0f;
    public float PunchStunSeconds = 1.0f;

    public List<FighterPunch.PunchEvent> Punches = new List<FighterPunch.PunchEvent>();


    // Private

    private static GlobalFightState instance;

    private List<GameObject> fighters = new List<GameObject>();


    // Events

    private void Awake()
    {
        instance = this;

        foreach (var fighter in GameObject.FindGameObjectsWithTag(PLAYER_CAPSULE_TAG))
        {
            AddFighter(fighter);
        }
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
            var knockVelocity = punch.TargetPosition - punch.SourcePosition;
            knockVelocity.y = PunchUpwardsForce;
            knockVelocity.Normalize();
            knockVelocity *= PunchForceMultiplier;

            var targetBody = punch.Target.GetComponent<Rigidbody>();
            targetBody.velocity = knockVelocity;

            var stun = punch.Target.GetComponent<FightStun>();
            stun.Stun(PunchStunSeconds);
        }

        // Setup for next frame
        Punches.Clear();
    }
}
