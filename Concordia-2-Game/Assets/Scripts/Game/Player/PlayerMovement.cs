using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IInputConsumer, IPunchable
{
    [Range(0.0f, 300.0f)]
    public float MovementSpeed;

    [Range(0.0f, 50.0f)]
    public float MaxMovementSpeed;

    [Range(0.0f, 1.0f)]
    public float MovementRotationSpeed;

    [Range(0.0f, 50.0f)]
    public float Gravity;

    [Range(0.0f, 1.0f)]
    public float FacingRotationSpeed;

    private Vector3 movementDirection = new Vector3();
    FightStun m_stun;
    Rigidbody m_rb;

    private bool m_movementIsInverted = false;

    private AudioSource audioSource;

    private AnimControl m_anim;
    private bool m_running = false;
    private float m_runSpeed = 1.0f;

    void Start()
    {
        m_stun = GetComponent<FightStun>();
        m_rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        m_anim = GetComponentInChildren<AnimControl>();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ID.HORIZONTAL))
        {
            float joystick = input.m_axisValue;
            movementDirection.x = joystick;
            return true;
        }
        if (input.GetActionID().Equals(con2.GamepadAction.ID.VERTICAL))
        {
            float joystick = input.m_axisValue;
            movementDirection.z = joystick;
            return true;
        }
        return false;
    }

    private void Update()
    {
        m_anim.SetRunning(m_running);
        m_anim.SetRunSpeed(m_runSpeed);
        m_anim.SetDizzy(m_stun.getMovementModifier() < 0.5f || (!m_running && m_stun.getMovementModifier() < 0.99f));
    }

    private void FixedUpdate()
    {
        movementDirection.y = 0.0f;

        if (movementDirection.sqrMagnitude > 1)
        {
            movementDirection.Normalize();
        }

        movementDirection *= MovementSpeed;

        // Apply stun factor
        movementDirection *= m_stun.getMovementModifier();

        if(m_movementIsInverted)
        {
            movementDirection *= -1;
        }

        // If player asked for input
        if (!Mathf.Approximately(movementDirection.magnitude, 0.0f))
        {
            m_rb.AddForce(movementDirection * MovementSpeed * MovementRotationSpeed * Time.deltaTime, ForceMode.Acceleration);

            // Cap movement speed
            if (m_rb.velocity.magnitude > MaxMovementSpeed)
            {
                m_rb.velocity = Vector3.ClampMagnitude(m_rb.velocity, MaxMovementSpeed);
            }
        }

        m_running = movementDirection.magnitude > 0.0f;
        m_runSpeed = Mathf.Lerp(0.5f, 1.0f, m_rb.velocity.magnitude / MaxMovementSpeed);

        // Gravity
        m_rb.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);

        // Smooth rotation according to velocity
        bool nonZeroInput = movementDirection.magnitude > 0.1f;
        if (nonZeroInput)
        {
            Vector3 targetRotation = movementDirection.normalized;

            Quaternion facing = new Quaternion();
            facing.SetLookRotation(targetRotation);
            facing = Quaternion.Slerp(transform.rotation, facing, FacingRotationSpeed);

            transform.rotation = facing;
        }

        movementDirection.x = 0.0f;
        movementDirection.y = 0.0f;
        movementDirection.z = 0.0f;
    }

    public void Punch(Vector3 knockVelocity, float stunTime)
    {
        m_rb.AddForce(knockVelocity, ForceMode.VelocityChange);
        m_stun.Stun(stunTime);
        audioSource.Play();
    }

    // Public API
    public void InvertMovement()
    {
        m_movementIsInverted = !m_movementIsInverted;
    }

    public void ModulateMovementDrag(float dragFraction)
    {
        m_rb.drag *= dragFraction;  
    }

    public void ModulateMovementSpeed(float movementModulator)
    {
        MovementSpeed *= movementModulator;
    }

    public void ModulateMaxMovementSpeed(float movementModulator)
    {
        MaxMovementSpeed *= movementModulator;
    }

    public void ModulateRotationSpeed(float rotationModulator)
    {
        FacingRotationSpeed /= rotationModulator;
        MovementRotationSpeed /= rotationModulator;
    }
}
