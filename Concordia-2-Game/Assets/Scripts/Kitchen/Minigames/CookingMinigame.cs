using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CookingMinigame : MonoBehaviour
{
    public const float INTERACTION_DISTANCE = 4.0f, FACING_DEGREE = 30.0f;

    [SerializeField]
    private Canvas m_prompt;
    private Vector3 stationLocation;

    [SerializeField]
    protected bool m_done;
    protected PlayerRange[] m_players;
    protected List<Gamepad> m_playerGamepads;

    public abstract void StartMinigameSpecifics();
    public abstract void UpdateMinigameSpecifics();
    public abstract void EndMinigameSpecifics();

    // Start is called before the first frame update
    private void Start()
    {
        stationLocation = this.transform.position;
        EndMinigame();
        FindPlayers();
    }

    private void FindPlayers()
    {
        GameObject managerObject = GameObject.FindGameObjectWithTag("PlayerManager");
        if (managerObject)
        {
            PlayerManager manager = managerObject.GetComponent<PlayerManager>();
            if (manager)
            {
                GameObject[] players = manager.GetPlayers();
                m_players = new PlayerRange[players.Length];
                for (int i = 0; i < players.Length; i++)
                {
                    m_players[i] = new PlayerRange(players[i]);
                    m_playerGamepads.Add(GamepadMgr.Pad(players[i].GetComponent<FightControls>().PlayerIndex));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_players == null)
        {
            FindPlayers();
        }
        else if (!m_done && CheckPlayersNear())
        {
            UpdateMinigameSpecifics();
        }
    }

    private bool CheckPlayersNear()
    {
        bool playersNear = false;
        foreach (PlayerRange player in m_players)
        {
            player.isInRange = false;
            Transform playerTransform = player.GetPlayer().transform;
            float distanceToKitchenStation = (playerTransform.position - stationLocation).magnitude;
            if (distanceToKitchenStation <= INTERACTION_DISTANCE && CheckPlayerFacingStation(playerTransform))
            {
                playersNear = true;
                player.isInRange = true;
            }
        }
        return playersNear;
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

    public class PlayerRange
    {
        private GameObject playerGameObject;
        public bool isInRange;

        public PlayerRange(GameObject player)
        {
            this.playerGameObject = player;
            isInRange = false;
        }

        public GameObject GetPlayer()
        {
            return playerGameObject;
        }
    }
}
