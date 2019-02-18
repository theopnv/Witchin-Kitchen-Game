using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IInputConsumer
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

    private Vector3 movementDirection = new Vector3();
    FightStun m_stun;
    Rigidbody m_rb;

    void Start()
    {
        m_stun = GetComponent<FightStun>();
        m_rb = GetComponent<Rigidbody>();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.MAX_ID)) //joystick movement
        {
            // Movement
            Vector2 joystick = input.m_movementDirection;
            movementDirection.x = joystick.x;
            movementDirection.y = 0.0f;
            movementDirection.z = joystick.y;
            
            if (movementDirection.sqrMagnitude > 1)
            {
                movementDirection.Normalize();
            }

            movementDirection *= MovementSpeed;
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        // Apply stun factor
        movementDirection *= m_stun.getMovementModifier();

        Vector3 bodyVelocityXZ = m_rb.velocity;
        bodyVelocityXZ.y = 0.0f;

        // Force wake-up rigid body
        m_rb.WakeUp();

        // Friction in XZ only
        if (!Mathf.Approximately(0.0f, bodyVelocityXZ.magnitude))
        {
            m_rb.velocity -= MovementFriction * Time.deltaTime * bodyVelocityXZ;
        }

        // If player asked for input
        if (!Mathf.Approximately(movementDirection.magnitude, 0.0f))
        {
            // Cap movement speed
            var requestedMovement = m_rb.velocity + movementDirection * Time.fixedDeltaTime;
            var requestedMovementXZ = bodyVelocityXZ + movementDirection * Time.fixedDeltaTime;
            var projected = Vector3.Project(requestedMovementXZ, m_rb.velocity);
            if (projected.magnitude > MaxMovementSpeed)
            {
                var acceptableFraction = Mathf.Max(0.0f, MaxMovementSpeed - projected.magnitude);
                requestedMovement = m_rb.velocity + movementDirection.normalized * acceptableFraction;
            }

            m_rb.velocity = requestedMovement;

            movementDirection.x = 0.0f;
            movementDirection.y = 0.0f;
            movementDirection.z = 0.0f;
        }

        // Gravity
        m_rb.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);

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
            facing = Quaternion.Slerp(transform.rotation, facing, RotationSpeed);

            transform.rotation = facing;
        }

        movementDirection.x = 0.0f;
        movementDirection.y = 0.0f;
        movementDirection.z = 0.0f;
    }
}
