using con2;
using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public abstract class ACookingMinigame : MonoBehaviour, IInputConsumer
    {
        public const float INTERACTION_DISTANCE = 2.5f, FACING_DEGREE = 45.0f;

        [SerializeField]
        protected Image m_promptBackground;
        [SerializeField]
        protected GameObject m_prompt;

        protected AMainManager m_mainManager;

        [HideInInspector]
        public PlayerManager Owner;
        public KitchenStation KitchenStation;
        private PlayerPickUpDropObject m_playerHolding;

        [SerializeField]
        protected bool m_started = false;

        //For any extra specifics a minigame requires at start, in update (e.g. a timer), and at end (e.g. spit out some item)
        public abstract void StartMinigameSpecifics();
        public abstract void BalanceMinigame();
        public abstract void UpdateMinigameSpecifics();
        public abstract void EndMinigameSpecifics();

        //Pass player input on to the non-abstract minigame if the player is in range and facing the station
        public abstract bool TryToConsumeInput(GamepadAction input);

        private void Awake()
        {
            m_mainManager = FindObjectOfType<AMainManager>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            m_promptBackground.enabled = false;
            m_prompt.SetActive(false);
            m_playerHolding = Owner.gameObject.GetComponentInChildren<PlayerPickUpDropObject>();
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
            float distanceToKitchenStation = (playerTransform.position - transform.position).magnitude;
            if (distanceToKitchenStation <= INTERACTION_DISTANCE && CheckPlayerFacingStation(playerTransform))
            {
                return true;
            }
            return false;
        }

        private bool CheckPlayerFacingStation(Transform player)
        {
            Vector3 playerFacing = player.TransformDirection(Vector3.forward);
            Vector3 playerToStation = transform.position - player.position;
            double currentAngle = Math.Acos(Vector3.Dot(playerFacing, playerToStation) / (playerFacing.magnitude * playerToStation.magnitude));
            return currentAngle <= ToRadian(2 * FACING_DEGREE);
        }

        private static double ToRadian(double degreeAngle)
        {
            return (Math.PI / 180.0) * degreeAngle;
        }

        public void StartMinigame()
        {
            BalanceMinigame();
            StartMinigameSpecifics();

            //Display prompt
            m_promptBackground.enabled = true;
            m_prompt.SetActive(true);

            m_started = true;
        }

        public void EndMinigame()
        {
            KitchenStation.ProcessIngredient();

            //Hide prompt
            m_promptBackground.enabled = false;
            m_prompt.SetActive(false);

            m_started = false;

            EndMinigameSpecifics();
        }

        public bool ConsumeInput(GamepadAction input)
        {
            var interactingPlayer = m_mainManager.GetPlayerById(input.GetPlayerId());
            var noOwner = Owner == null;
            var samePlayer = Owner.ID == interactingPlayer.ID;
            if (m_started && (noOwner || samePlayer) && CheckPlayerIsNear(interactingPlayer.gameObject))
            {
                if (!noOwner && m_playerHolding.IsHoldingObject())
                {
                    return false;
                }
                return TryToConsumeInput(input);
            }
            return false;
        }
    }

}
