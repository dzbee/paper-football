using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGoalFlick : MonoBehaviour
{
    [SerializeField] Referee referee;
    Rigidbody footballBody;
    float force = 0f;
    [SerializeField] float powerSpeed = 1f;
    [SerializeField] float flickDisplacement = 0.45f;
    public bool scoringPosition;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Flick(Referee.Player player){
        var cornerOffset = flickDisplacement * Vector3.down;
        var position = Vector3.zero;
        var direction = Vector3.zero;
        switch (player) {
            case Referee.Player.Player1:
                position = transform.position + cornerOffset;
                direction = Vector3.up + Vector3.forward;
                break;
            case Referee.Player.Player2:
                position = transform.position - cornerOffset;
                direction = Vector3.up + Vector3.back;
                break;                
            case Referee.Player.Computer:
                position = transform.position - cornerOffset;
                direction = Vector3.up + Vector3.back;
                force = 1f;
                break;
        }
        footballBody.AddForceAtPosition(
                force * direction.normalized,
                position,
                ForceMode.Impulse
            );
        force = 0f;
        StartCoroutine(referee.WaitAndUpdateState());
    }

    void Awake() {
        footballBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnEnable(){
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    void Update() {
        if (Input.GetButton("Jump") & referee.playState == Referee.PlayState.FGAttempt) {
            force += powerSpeed * Time.deltaTime;
        } else if (Input.GetButtonUp("Jump") & referee.playState == Referee.PlayState.FGAttempt) {
            if (referee.PlayerTurn() == Referee.Player.Player1) {
                Flick(Referee.Player.Player1);
            } else if (referee.PlayerTurn() == Referee.Player.Player2) {
                Flick(Referee.Player.Player2);
            }
        }

        if (referee.PlayerTurn() == Referee.Player.Computer & referee.playState == Referee.PlayState.FGAttempt) {
            Flick(Referee.Player.Computer);
        }
    }

    bool ScoringPosition(Collider other, Referee.Player player){
        return (other.CompareTag("PlayerFG") & player != Referee.Player.Player1) |
               (other.CompareTag("OppoFG") & player == Referee.Player.Player1);
    }

    void OnTriggerStay(Collider other){
        if(ScoringPosition(other, referee.PlayerTurn())){
            scoringPosition = true;
            footballBody.Sleep();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerFG") | other.CompareTag("OppoFG")) {
            scoringPosition = false;
        }
    }
}
