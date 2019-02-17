using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class FightControls : MonoBehaviour
    {
        [Range(0.0f, 300.0f)]
        public float MovementSpeed;

        [Range(0.0f, 50.0f)]
        public float MaxMovementSpeed;

        [Range(0.0f, 50.0f)]
        public float MovementFriction;

        [Range(0.0f, 50.0f)]
        public float Gravity;

        [Range(0.0f, 1.0f)]
        public float RotationSpeed;

        private GameObject target;
        private FighterPunch punch;
        private bool canPunch = true;
        private int playerIndex;

        private Vector3 movementDirection = new Vector3();

        // Start is called before the first frame update
        void Start()
        {
            target = gameObject;
            punch = target.GetComponentInChildren<FighterPunch>();
        }

        // Update is called once per frame
        void Update()
        {
            // Movement
            var joystick = con2.GamepadMgr.Pad(playerIndex).MovementDirectionRaw();
            movementDirection.x = joystick.x;
            movementDirection.y = 0.0f;
            movementDirection.z = joystick.y;

            if (movementDirection.sqrMagnitude > 1)
            {
                movementDirection.Normalize();
            }
            movementDirection *= MovementSpeed;

            // Punch
            if (canPunch && con2.GamepadMgr.Pad(playerIndex).Action(con2.GamepadAction.ID.PUNCH).JustPressed)
            {
                canPunch = false;
                punch.RequestPunch();

                // Cancel punch after some time
                StartCoroutine(StopPunch());

                // Reload punch after some time
                StartCoroutine(ReloadPunch());
            }
        }

        IEnumerator StopPunch()
        {
            yield return new WaitForSeconds(GlobalFightState.get().PunchLingerSeconds);
            punch.CancelPunch();
        }

        IEnumerator ReloadPunch()
        {
            yield return new WaitForSeconds(GlobalFightState.get().PunchReloadSeconds);
            canPunch = true;
        }

        private void FixedUpdate()
        {
            // Apply stun factor
            var stun = target.GetComponent<FightStun>();
            movementDirection *= stun.getMovementModifier();

            var body = target.GetComponent<Rigidbody>();
            var bodyVelocityXZ = body.velocity;
            bodyVelocityXZ.y = 0.0f;

            // Force wake-up rigid body
            body.WakeUp();

            // Friction in XZ only
            if (!Mathf.Approximately(0.0f, bodyVelocityXZ.magnitude))
            {
                body.velocity -= MovementFriction * Time.deltaTime * bodyVelocityXZ;
            }

            // If player asked for input
            if (!Mathf.Approximately(movementDirection.magnitude, 0.0f))
            {
                // Cap movement speed
                var requestedMovement = body.velocity + movementDirection * Time.fixedDeltaTime;
                var requestedMovementXZ = bodyVelocityXZ + movementDirection * Time.fixedDeltaTime;
                var projected = Vector3.Project(requestedMovementXZ, body.velocity);
                if (projected.magnitude > MaxMovementSpeed)
                {
                    var acceptableFraction = Mathf.Max(0.0f, MaxMovementSpeed - projected.magnitude);
                    requestedMovement = body.velocity + movementDirection.normalized * acceptableFraction;
                }

                body.velocity = requestedMovement;
            }

            // Gravity
            body.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);

            // Smooth rotation according to velocity
            bool nonZeroVelocity = bodyVelocityXZ.magnitude > 0.1f;
            bool nonZeroInput = movementDirection.magnitude > 0.1f;
            if (nonZeroVelocity || nonZeroInput)
            {
                Vector3 targetRotation = bodyVelocityXZ.normalized;

                // Override by player's desired rotation, if any
                if (nonZeroInput)
                {
                    targetRotation = movementDirection.normalized;
                }

                Quaternion facing = new Quaternion();
                facing.SetLookRotation(targetRotation);
                facing = Quaternion.Slerp(target.transform.rotation, facing, RotationSpeed);

                target.transform.rotation = facing;
            }
        }

        public void SetPlayerIndex(int newIndex)
        {
            playerIndex = newIndex;
        }
    }

}
