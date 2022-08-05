using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGoalFlick : MonoBehaviour
{
    public Referee referee;
    public FootballMovement primaryFootball;
    private Rigidbody footballBody;
    private float force = 0f;
    public float powerSpeed = 1f;
    public float flickDisplacement = 0.45f;
    private bool kicked = false;
    private bool exited = false;
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
        kicked = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        footballBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnEnable(){
        referee.SetPlayState(Referee.PlayState.Playing);
        kicked = false;
        exited = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Jump") & !kicked) {
            force += powerSpeed * Time.deltaTime;
        } else if (Input.GetButtonUp("Jump") & !kicked) {
            if (referee.PlayerTurn() == Referee.Player.Player1) {
                Flick(Referee.Player.Player1);
            } else if (referee.PlayerTurn() == Referee.Player.Player2) {
                Flick(Referee.Player.Player2);
            }
        }

        if(referee.PlayerTurn() == Referee.Player.Computer & !kicked){
            Flick(Referee.Player.Computer);
        }

        if(kicked & (footballBody.IsSleeping() | referee.OutOfBounds()) & !exited){
            exited = true;
            StartCoroutine(referee.ResetPlay());
        }
    }

    bool ScoringPosition(Collider other, Referee.Player player){
        return (other.CompareTag("PlayerFG") & (player == Referee.Player.Player2 | player == Referee.Player.Computer)) |
               (other.CompareTag("OppoFG") & player == Referee.Player.Player1);
    }

    void OnTriggerStay(Collider other){
        if(ScoringPosition(other, referee.PlayerTurn()) & referee.InPlay()){
            referee.SetPlayState(Referee.PlayState.Stopped);
            referee.ExtraPoint(referee.PlayerTurn());
            footballBody.velocity = Vector3.zero;
            footballBody.Sleep();
        }
    }
}
