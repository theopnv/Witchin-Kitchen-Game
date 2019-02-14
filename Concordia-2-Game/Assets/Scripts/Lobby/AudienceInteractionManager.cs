using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class AudienceInteractionManager : MonoBehaviour
{
    private SocketIOComponent _Socket;

    // Start is called before the first frame update
    void Start()
    {
        _Socket = GetComponent<SocketIOComponent>();

        // Small delay between object instantiation and first use.
        StartCoroutine("Authenticate");
    }

    private IEnumerator Authenticate()
    {
        yield return new WaitForSeconds(1);
        _Socket.Emit("registerAsHost");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
