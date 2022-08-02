using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FootballMovement : MonoBehaviour
{
    public Rigidbody footballBody;
    public TextMeshProUGUI scoreboard;
    public float flickDisplacement = 0.45f;
    public float powerSpeed = 10f;
    private float force = 0f;
    private bool onTable = true;
    private bool inPlay = true;
    private bool isKickoff = true;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 cornerOffset;
    private int playerCount = 0;
    private int opponentCount = 0;
    private bool playerDrive = true;

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

    void SetScore()
    {
        scoreboard.text = "Player: " + (6 * playerCount).ToString() + "\nOpponent: " + (6 * opponentCount).ToString();
    }

    void OnTriggerStay(Collider other)
    {
        if(footballBody.IsSleeping() && inPlay){
            inPlay = false;
            if(playerDrive && other.CompareTag("OppoEnd")){
                playerCount++;
                SetScore();
                StartCoroutine(reset());
            } else if(!playerDrive && other.CompareTag("PlayerEnd")){
                opponentCount++;
                SetScore();
                StartCoroutine(reset());
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("PlayerEnd") | other.gameObject.CompareTag("OppoEnd")){
            inPlay = true;
        }
    }


    void Update()
    {
        if(Input.GetButton("Jump") & onTable) {
            force += powerSpeed * Time.deltaTime;
        } else if (Input.GetButtonUp("Jump") & isKickoff) {
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

        if(transform.position.y < 0f){
            StartCoroutine(reset());
        }
    }

    IEnumerator reset() {
        yield return new WaitForSeconds(0.5f);
        transform.position = startPosition;
        transform.rotation = startRotation;
        isKickoff = true;
    }
}
