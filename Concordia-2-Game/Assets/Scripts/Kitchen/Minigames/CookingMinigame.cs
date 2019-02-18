using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CookingMinigame : MonoBehaviour, IInputConsumer
{
    public const float INTERACTION_DISTANCE = 4.0f, FACING_DEGREE = 30.0f;

    [SerializeField]
    private Canvas m_prompt;
    private Vector3 stationLocation;
    private GameObject m_stationOwner;

    [SerializeField]
    protected bool m_done;

    //For any extra specifics a minigame requires at start, in update (e.g. a timer), and at end (e.g. spit out some item)
    public abstract void StartMinigameSpecifics();
    public abstract void UpdateMinigameSpecifics();
    public abstract void EndMinigameSpecifics();

    //Pass player input on to the non-abstract minigame if the player is in range and facing the station
    public abstract bool TryToConsumeInput(GamepadAction input);

    // Start is called before the first frame update
    private void Start()
    {
        stationLocation = this.transform.position;
        EndMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_done)
        {
            UpdateMinigameSpecifics();
        }
    }

    protected bool CheckPlayerIsNear(GameObject player)
    {
        Transform playerTransform = player.transform;
        float distanceToKitchenStation = (playerTransform.position - stationLocation).magnitude;
        if (distanceToKitchenStation <= INTERACTION_DISTANCE && CheckPlayerFacingStation(playerTransform))
        {
            return true;
        }
        return false;
    }

    private bool CheckPlayerFacingStation(Transform player)
    {
        Vector3 playerFacing = player.TransformDirection(Vector3.forward);
        Vector3 playerToStation = stationLocation - player.position;
        return Vector3.Dot(playerFacing, playerToStation) <= FACING_DEGREE;
    }

    public void StartMinigame()
    {
        //Display prompt
        m_prompt.gameObject.SetActive(true);

        StartMinigameSpecifics();
    }

    public void EndMinigame()
    {
        //Hide prompt
        m_prompt.gameObject.SetActive(false);

        //Reset progress
        m_done = false;

        EndMinigameSpecifics();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        GameObject interactingPlayer = input.GetPlayer();
        if ((m_stationOwner == null || m_stationOwner.Equals(interactingPlayer)) && CheckPlayerIsNear(interactingPlayer))
        {
            return TryToConsumeInput(input);
        }
        return false;
    }

    public void SetStationOwner(GameObject owner)
    {
        m_stationOwner = owner;
    }
}
