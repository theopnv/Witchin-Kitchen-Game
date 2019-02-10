using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 movementDirection = new Vector3();
    private Vector3 facingDirection = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        target = gameObject;

        // Snap to initial object rotation
        facingDirection.x = target.transform.forward.x;
        facingDirection.z = target.transform.forward.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = 0.0f;
        movementDirection.z = Input.GetAxisRaw("Vertical");

        if (movementDirection.sqrMagnitude > 1)
        {
            movementDirection.Normalize();
        }
        movementDirection *= MovementSpeed;

        // Rotation
        var playerOnScreen = Camera.main.WorldToScreenPoint(target.transform.position);
        var mouseOnScreen = Input.mousePosition;

        facingDirection = mouseOnScreen - playerOnScreen;
        facingDirection.Normalize();
        facingDirection.z = facingDirection.y;
        facingDirection.y = 0.0f;
    }

    private void FixedUpdate()
    {
        // Apply stun factor
        var stun = target.GetComponent<FightStun>();
        movementDirection *= stun.getMovementModifier();

        var body = target.GetComponent<Rigidbody>();
        var bodyVelocityXZ = body.velocity;
        bodyVelocityXZ.y = 0.0f;

        // Friction in XZ only
        body.velocity -= MovementFriction * Time.deltaTime * bodyVelocityXZ;

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

        // Smooth rotation
        Quaternion facing = new Quaternion();
        facing.SetLookRotation(facingDirection);
        facing = Quaternion.Slerp(target.transform.rotation, facing, RotationSpeed);

        target.transform.rotation = facing;
    }
}
