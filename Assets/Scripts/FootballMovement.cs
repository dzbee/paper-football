using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballMovement : MonoBehaviour
{
    public Rigidbody footballBody;
    public float flickDisplacement = 0.45f;
    public float powerSpeed = 10f;
    private float force = 0f;
    private bool onTable = true;
    private bool isKickoff = true;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 cornerOffset;

    void Start()
    {
        footballBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        cornerOffset = startPosition - flickDisplacement * (transform.forward - transform.right);
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
        if(Input.GetButton("Jump") & onTable) {
            force += powerSpeed * Time.deltaTime;
        } else if (Input.GetButtonUp("Jump") & isKickoff) {
            onTable = false;
            footballBody.AddForceAtPosition(
                force * (Vector3.up + Vector3.forward), 
                transform.position + cornerOffset, 
                ForceMode.Impulse
            );
            force = 0f;
            isKickoff = false;
        } else if (Input.GetButtonUp("Jump") & !isKickoff) {
            footballBody.AddForce(force * Vector3.forward, ForceMode.Impulse);
            force = 0f;
        }

        if(transform.position.y < -1f){
            transform.position = startPosition;
            transform.rotation = startRotation;
            isKickoff = true;
        }
    }
}
