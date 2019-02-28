using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    #region Private Attributes

    private float Speed = 350f;

    private bool _HasHitTheGround;
    
    private Vector3 _Direction;

    private Rigidbody _Rb;

    private Material _Material;

    #endregion

    void Start()
    {
        _Rb = GetComponent<Rigidbody>();
        if (!_Rb)
        {
            Debug.LogError("Could not find Rigidbody on Meteor");
        }

        _Rb.AddTorque(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f));

        _Material = GetComponentInChildren<MeshRenderer>().material;
    }

    public void SetDirection(Vector3 target)
    {
        _Direction = target - transform.position;
    }

    void FixedUpdate()
    {
        if (_Material.color.a <= 0)
        {
            Destroy(gameObject);
        }

        if (_Direction != null && !_HasHitTheGround)
        {
            _Rb.AddForce(_Direction.normalized * Time.deltaTime * Speed);
        }

        if (_HasHitTheGround)
        {
            var color = _Material.color;
            color.a -= 1 * Time.deltaTime;
            _Material.color = color;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ArenaBase")
        {
            _HasHitTheGround = true;
            _Rb.velocity = Vector3.zero;
            _Rb.freezeRotation = true;
            
        }
    }
}
