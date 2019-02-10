using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightControls : MonoBehaviour
{
    public float MovementSpeed;

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
        movementDirection.x = Input.GetAxis("Horizontal");
        movementDirection.y = 0.0f;
        movementDirection.z = Input.GetAxis("Vertical");

        movementDirection.Normalize();
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
        var body = target.GetComponent<Rigidbody>();

        body.velocity += movementDirection * Time.fixedDeltaTime;

        // Smooth rotation
        Quaternion facing = new Quaternion();
        facing.SetLookRotation(facingDirection);
        facing = Quaternion.Slerp(target.transform.rotation, facing, RotationSpeed);

        target.transform.rotation = facing;
    }
}
