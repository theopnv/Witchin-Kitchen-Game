using con2;
using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ACookingMinigame : MonoBehaviour, IInputConsumer
{
    public const float INTERACTION_DISTANCE = 2.0f, FACING_DEGREE = 30.0f;

    [SerializeField]
    protected Text m_prompt;
    private Vector3 stationLocation;
    public GameObject m_stationOwner;
    private KitchenStation m_kitchenStation;
    private SpawnPlayersController m_playerSpawner;

    [SerializeField]
    protected bool m_started = false;

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
        m_prompt.enabled = false;
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        m_playerSpawner = managers.GetComponentInChildren<SpawnPlayersController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_started)
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
        double currentAngle = Math.Acos(Vector3.Dot(playerFacing, playerToStation) / (playerFacing.magnitude * playerToStation.magnitude));
        return currentAngle <= ToRadian(2*FACING_DEGREE);
    }

    private static double ToRadian(double degreeAngle)
    {
        return (Math.PI / 180.0) * degreeAngle;
    }

    public void StartMinigame()
    {
        //Display prompt
        m_prompt.enabled = true;

        m_started = true;

        StartMinigameSpecifics();
    }

    public void EndMinigame()
    {
        m_kitchenStation.ProcessIngredient();

        //Hide prompt
        m_prompt.enabled = false;

        m_started = false;

        EndMinigameSpecifics();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        GameObject interactingPlayer = m_playerSpawner.GetPlayerByID(input.GetPlayerId());
        bool noOwner = m_stationOwner == null;
        bool samePlayer = m_stationOwner.Equals(interactingPlayer);
        if (m_started && (noOwner || samePlayer) && CheckPlayerIsNear(interactingPlayer))
        {
            return TryToConsumeInput(input);
        }
        return false;
    }

    public void SetStationOwner(GameObject owner, KitchenStation kitchenStation)
    {
        m_kitchenStation = kitchenStation;
        m_stationOwner = owner;
    }

    public GameObject GetStationOwner()
    {
        return m_stationOwner;
    }
}
