using UnityEngine;

// Author: Tri-Luong Steven Dien
public class Player_PickUpDropObject : MonoBehaviour
{
    [Header("Player Hands")]
    public Transform mCharacterHands;
    public Transform mCharacterFeet;
    public float mThrowForce;
    private float mPlayerHeight;
    private Rigidbody mPlayerRB;

    // The actual pickable object
    private PickableObject mPickableObject;

    // The object mass
    private float mExtraWeight;

    void Start()
    {
        mPlayerHeight = GetComponent<Collider>().bounds.size.y;
        mPlayerRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Pick up / drop nearby item
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("joystick button 2"))
        {
            if (IsHoldingObject())
                DropObject();
            else if (GetNearestItem())
                PickUpObject();
        }

        if (IsHoldingObject())
        {
            // Keeps the object in hands at the same position and orientation
            mPickableObject.UpdatePosition(mCharacterHands.localPosition);
        }
    }

    private bool GetNearestItem()
    {
        RaycastHit rayItemHit;

        // Check if an pickable object is in range
        if (Physics.Raycast(transform.position - new Vector3(0, mPlayerHeight / 2.5f, 0), transform.TransformDirection(Vector3.forward), out rayItemHit, 1f))
        {
            mPickableObject = rayItemHit.transform.gameObject.GetComponent<PickableObject>();
            if (mPickableObject)
            {
                mExtraWeight = rayItemHit.transform.gameObject.GetComponent<Rigidbody>().mass;
                return true;
            }
        }
        return false;
    }

    // Pick up a nearby object
    private void PickUpObject()
    {
        // Add the weight of the object to the total weight of the player
        GetComponent<Rigidbody>().mass += mExtraWeight;

        // Have the object adjust its physics
        mPickableObject.PickUp(transform);

        // Reposition the player hands (location)
        //mCharacterHands.localPosition = new Vector3(0.0f, playerSize.y + objectSize.y / 2.0f, 0.0f);
    }

    // Drop the object in hands
    private void DropObject()
    {

        // Remove the object weight from the player total weight
        mPlayerRB.mass -= mExtraWeight;

        // Have the object adjust its physics and get thrown
        mPickableObject.Drop(mPlayerRB.velocity + transform.forward*mThrowForce);

        // Reset picked up object values
        mExtraWeight = 0.0f;
        mPickableObject = null;
    }

    // Get the value of mIsHoldingObject
    public bool IsHoldingObject()
    {
        return mPickableObject;
    }
}
