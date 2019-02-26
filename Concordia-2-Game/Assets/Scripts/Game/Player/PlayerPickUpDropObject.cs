using con2;
using UnityEngine;

public class PlayerPickUpDropObject : MonoBehaviour, IInputConsumer, IPunchable
{
    public Transform m_characterHands;
    public float m_throwForce = 5;
    private float m_playerHeight;
    private Rigidbody m_playerRB;
    private PlayerMovement m_playerMovement;

    // The actual pickable object
    private PickableObject m_pickableObject;

    void Start()
    {
        m_playerHeight = GetComponent<Collider>().bounds.size.y;
        m_playerRB = GetComponent<Rigidbody>();
        m_playerMovement = GetComponent<PlayerMovement>();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.PUNCH))
        {
            if (IsHoldingObject())
                return true;
        }

        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.INTERACT))
        {
            if (IsHoldingObject())
                DropObject(m_playerRB.velocity + (transform.forward * m_throwForce));
            else if (GetNearestItem())
                PickUpObject();
            else
                return false;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (IsHoldingObject())
        {
            // Keeps the object in hands at the same position and orientation
            m_pickableObject.UpdatePosition(m_characterHands.localPosition);
        }
    }

    private bool GetNearestItem()
    {
        RaycastHit rayItemHit;

        // Check if an pickable object is in range
        if (Physics.Raycast(transform.position - new Vector3(0, m_playerHeight / 2.5f, 0), transform.TransformDirection(Vector3.forward), out rayItemHit, 1f))
        {
            m_pickableObject = rayItemHit.transform.gameObject.GetComponent<PickableObject>();
        }
        return m_pickableObject;
    }

    // Pick up a nearby object
    private void PickUpObject()
    {
        // Slow down the player
        m_playerMovement.MaxMovementSpeed *= m_pickableObject.GetMaxSpeedFractionWhenHolding();

        // Have the object adjust its physics
        m_pickableObject.PickUp(transform);

        // Reposition the player hands (location)
        //mCharacterHands.localPosition = new Vector3(0.0f, playerSize.y + objectSize.y / 2.0f, 0.0f);
    }

    // Drop the object in hands
    private void DropObject(Vector3 throwVector)
    {
        // Restore max movement speed
        m_playerMovement.MaxMovementSpeed /= m_pickableObject.GetMaxSpeedFractionWhenHolding();

        // Have the object adjust its physics and get thrown
        m_pickableObject.Drop(throwVector);

        // Reset picked up object
        m_pickableObject = null;
    }

    public void Punch(Vector3 knockVelocity, float stunTime)
    {
        if (IsHoldingObject())
        {
            Vector3 knockVector = -knockVelocity.normalized;
            DropObject(new Vector3(knockVector.x, knockVector.z) * m_throwForce);
        }
    }

    // Get the value of mIsHoldingObject
    public bool IsHoldingObject()
    {
        return m_pickableObject;
    }
}