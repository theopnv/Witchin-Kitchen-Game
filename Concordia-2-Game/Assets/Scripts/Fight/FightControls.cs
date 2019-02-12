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

    [Range(0, 3)]
    public int PlayerIndex;

    public bool UseMouse = true;

    private GameObject target;
    private FighterPunch punch;
    private bool canPunch = true;

    private Vector3 movementDirection = new Vector3();
    private Vector3 facingDirection = new Vector3();

    private static string INPUT_HORIZONTAL = "Horizontal";
    private static string INPUT_VERTICAL   = "Vertical";
    private static string INPUT_HORIZONTAL_RIGHT = "Horizontal Right";
    private static string INPUT_VERTICAL_RIGHT = "Vertical Right";
    private static string INPUT_PUNCH      = "Punch";

    // Start is called before the first frame update
    void Start()
    {
        target = gameObject;
        punch = target.GetComponentInChildren<FighterPunch>();

        // Snap to initial object rotation
        facingDirection.x = target.transform.forward.x;
        facingDirection.z = target.transform.forward.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        movementDirection.x = Input.GetAxisRaw(getInputStringForPlayer(INPUT_HORIZONTAL, PlayerIndex));
        movementDirection.y = 0.0f;
        movementDirection.z = Input.GetAxisRaw(getInputStringForPlayer(INPUT_VERTICAL, PlayerIndex));

        if (movementDirection.sqrMagnitude > 1)
        {
            movementDirection.Normalize();
        }
        movementDirection *= MovementSpeed;

        // Rotation
        if (UseMouse)
        {
            var playerOnScreen = Camera.main.WorldToScreenPoint(target.transform.position);
            var mouseOnScreen = Input.mousePosition;

            facingDirection = mouseOnScreen - playerOnScreen;
            facingDirection.Normalize();
            facingDirection.z = facingDirection.y;
            facingDirection.y = 0.0f;
        }
        else
        {
            facingDirection.x = Input.GetAxisRaw(getInputStringForPlayer(INPUT_HORIZONTAL_RIGHT, PlayerIndex));
            facingDirection.y = 0.0f;
            facingDirection.z = Input.GetAxisRaw(getInputStringForPlayer(INPUT_VERTICAL_RIGHT, PlayerIndex));
            facingDirection.Normalize();
        }

        // Punch
        if (canPunch && Input.GetButtonDown(getInputStringForPlayer(INPUT_PUNCH, PlayerIndex)))
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
        if (!Mathf.Approximately(0.0f, facingDirection.magnitude))
        {
            Quaternion facing = new Quaternion();
            facing.SetLookRotation(facingDirection);
            facing = Quaternion.Slerp(target.transform.rotation, facing, RotationSpeed);

            target.transform.rotation = facing;
        }
    }

    private string getInputStringForPlayer(string input, int idx)
    {
        // Idx is 0-based at runtime but 1-based in project settings
        return input + " P" + (idx + 1);
    }
}
