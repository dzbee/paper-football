using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flick : MonoBehaviour
{
    public Rigidbody footballBody;
    public float flickDisplacement = 0.45f;
    public float flickForce = 1f;
    private bool onTable = true;
    private Vector3 cornerOffset;

    void Start()
    {
        footballBody = GetComponent<Rigidbody>();
        cornerOffset = transform.position - flickDisplacement * (transform.forward - transform.right);
    }

    void OnCollisionStay()
    {
        onTable = true;
    }

    void OnCollisionExit()
    {
        onTable = false;
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") & onTable) {
            onTable = false;
            footballBody.AddForceAtPosition(
                flickForce * (Vector3.up + Vector3.forward), 
                transform.position + cornerOffset, 
                ForceMode.Impulse
            );
            Debug.Log("applied force at " + transform.position + cornerOffset);
        }
    }
}
