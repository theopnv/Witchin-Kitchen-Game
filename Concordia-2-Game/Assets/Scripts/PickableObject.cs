using UnityEngine;

public class PickableObject : MonoBehaviour
{
    // The object's rigidbody
    private Rigidbody rb;
    public float m_maxSpeedFractionWhenHolding = .85f;
    public Ingredient ingredientType = Ingredient.NOT_AN_INGREDIENT;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called by the carrying player's Update() to force the object to follow it
    public void UpdatePosition(Vector3 currentPos)
    {
        transform.localPosition = currentPos;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    // Get picked up
    public void PickUp(Transform newParent)
    {
        // Reset rotation
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.parent = newParent;

        // Disable the use of gravity, remove the velocity, and freeze rotation (will all be driven by player movement)
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    //Get dropped
    public void Drop(Vector3 throwVector)
    {
        // Re-Enable the use of gravity on the object and remove all constraints
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        // Get thrown forward
        rb.AddForce(throwVector, ForceMode.Impulse);

        // Unparent the object from the player
        transform.parent = null;
    }

    public float GetMaxSpeedFractionWhenHolding()
    {
        return m_maxSpeedFractionWhenHolding;
    }

}
