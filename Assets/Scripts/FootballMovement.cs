using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballMovement : MonoBehaviour
{
    public Rigidbody footballBody;
    public Referee referee;
    public GameObject playerEndzone;
    public GameObject opponentEndzone;
    public float flickDisplacement = 0.45f;
    public float powerSpeed = 10f;
    private float force = 0f;
    private Vector3 playerKickoffPosition;
    private Vector3 opponentKickoffPosition;
    private Quaternion startRotation;
    private Vector3 cornerOffset;
    public IEnumerator waitRoutine;
    private float waitTime;

    public IEnumerator SetupKickoff(Referee.Player player) {
        yield return new WaitForSeconds(1);
        if(player == Referee.Player.Player1){
            transform.position = playerKickoffPosition;
        } else {
            transform.position = opponentKickoffPosition;
        }
        transform.rotation = startRotation;
        footballBody.velocity = Vector3.zero;
        referee.ReadyForKickoff(true);
        referee.SetPlayState(Referee.PlayState.Playing);
    }

    void Kick(Referee.Player player){
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
                force = 0.45f;
                break;
        }
        footballBody.AddForceAtPosition(
                force * direction,
                position,
                ForceMode.Impulse
            );
        force = 0f;
        referee.ReadyForKickoff(false);
    }

    void Push(Referee.Player player){
        var direction = Vector3.zero;
        switch (player) {
            case Referee.Player.Player1:
                direction = Vector3.forward;
                break;
            case Referee.Player.Player2:
                direction = Vector3.back;
                break;
            case Referee.Player.Computer:
                direction = Vector3.back;
                force = 1f;
                break;
        }
        footballBody.AddForce(force * direction, ForceMode.Impulse);
        force = 0f;
    }

    void Play(Referee.Player player){
        if(referee.Kickable(player)){
            Kick(player);
        } else if(referee.Pushable(player)){
            Push(player);
        }
    }

    void Start()
    {
        footballBody = GetComponent<Rigidbody>();
        playerKickoffPosition = new Vector3(
            playerEndzone.transform.position.x,
            transform.position.y,
            playerEndzone.transform.position.z * .98f
        );
        opponentKickoffPosition = new Vector3(
            opponentEndzone.transform.position.x,
            transform.position.y,
            opponentEndzone.transform.position.z * .98f
        );
        startRotation = transform.rotation;
        cornerOffset = transform.position - flickDisplacement * (transform.forward - transform.right);
        if (referee.nPlayers == 2){
            waitTime = 0;
        } else {
            waitTime = 1;
        }
    }

    void Update()
    {
        if(Input.GetButton("Jump") & (referee.Playable(Referee.Player.Player1) | referee.Playable(Referee.Player.Player2))) {
            force += powerSpeed * Time.deltaTime;
        } else if (Input.GetButtonUp("Jump") & (referee.Playable(Referee.Player.Player1) | referee.Playable(Referee.Player.Player2))){
            if (referee.Playable(Referee.Player.Player1)) {
                Play(Referee.Player.Player1);
            } else {
                Play(Referee.Player.Player2);
            }
            referee.SetPlayState(Referee.PlayState.Waiting);
            if (waitRoutine != null) {
                StopCoroutine(waitRoutine);
            }
            waitRoutine = PlayEndAndWait(waitTime);
            StartCoroutine(waitRoutine);
        }

        if(referee.Playable(Referee.Player.Computer)){
            referee.SetPlayState(Referee.PlayState.Waiting);
            Play(Referee.Player.Computer);
            if (waitRoutine != null) {
                StopCoroutine(waitRoutine);
            }
            waitRoutine = PlayEndAndWait(0);
            StartCoroutine(waitRoutine);
        }

        if(referee.OutOfBounds() & referee.InPlay()){
            referee.SetPlayState(Referee.PlayState.Stopped);
            StopCoroutine(waitRoutine);
            StartCoroutine(SetupKickoff(referee.PlayerTurn()));
        }
    }

    bool ScoringPosition(Collider other, Referee.Player player){
        return (other.CompareTag("PlayerEnd") & (player == Referee.Player.Player2 | player == Referee.Player.Computer)) |
               (other.CompareTag("OppoEnd") & player == Referee.Player.Player1);
    }

    void OnTriggerStay(Collider other){
        if(ScoringPosition(other, referee.PlayerTurn()) & referee.InPlay()){
            StopCoroutine(waitRoutine);
        }
        if(ScoringPosition(other, referee.PlayerTurn()) & referee.Scoreable()){
            referee.SetPlayState(Referee.PlayState.Stopped);
            referee.Touchdown(referee.PlayerTurn());
        }
    }

    IEnumerator PlayEndAndWait(float seconds){
        yield return new WaitUntil(footballBody.IsSleeping);
        yield return new WaitForSeconds(seconds);
        referee.ChangeTurn();
        referee.SetPlayState(Referee.PlayState.Playing);
    }
}
