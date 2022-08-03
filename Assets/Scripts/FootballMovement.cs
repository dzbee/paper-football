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

    public IEnumerator SetupKickoff(Referee.Player player) {
        yield return new WaitForSeconds(1);
        switch (player) {
            case Referee.Player.Player1:
                transform.position = playerKickoffPosition;
                break;
            case Referee.Player.Player2:
                transform.position = opponentKickoffPosition;
                break;
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
                force = 0.45f;
                break;
        }
        Debug.Log("kick! force: " + force + "direction: " + direction + "position: " + position);
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
                force = 1f;
                break;
        }
        Debug.Log("push! force: " + force + "direction: " + direction);
        footballBody.AddForce(force * direction, ForceMode.Impulse);
        force = 0f;
    }

    void Play(Referee.Player player){
        Debug.Log("deciding play");
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
    }

    void Update()
    {
        if(Input.GetButton("Jump") & referee.Playable(Referee.Player.Player1)) {
            force += powerSpeed * Time.deltaTime;
        } else if (Input.GetButtonUp("Jump") & referee.Playable(Referee.Player.Player1)){
            Debug.Log("executing player turn");
            referee.SetPlayState(Referee.PlayState.Waiting);
            Play(Referee.Player.Player1);
            Debug.Log("waiting for play to end");
            if (waitRoutine != null) {
                StopCoroutine(waitRoutine);
            }
            waitRoutine = PlayEndAndWait(1);
            StartCoroutine(waitRoutine);
        }

        if(referee.Playable(Referee.Player.Player2)){
            Debug.Log("executing opponent turn");
            referee.SetPlayState(Referee.PlayState.Waiting);
            Play(Referee.Player.Player2);
            if (waitRoutine != null) {
                StopCoroutine(waitRoutine);
            }
            waitRoutine = PlayEndAndWait(0);
            StartCoroutine(waitRoutine);
        }

        if(referee.OutOfBounds() & referee.InPlay()){
            Debug.Log("stopping play for OOB");
            referee.SetPlayState(Referee.PlayState.Stopped);
            Debug.Log("halting wait coroutine");
            StopCoroutine(waitRoutine);
            Debug.Log("setting up kickoff");
            StartCoroutine(SetupKickoff(referee.PlayerTurn()));
        }
    }

    bool ScoringPosition(Collider other, Referee.Player player){
        return (other.CompareTag("PlayerEnd") & player == Referee.Player.Player2) |
               (other.CompareTag("OppoEnd") & player == Referee.Player.Player1);
    }

    void OnTriggerStay(Collider other){
        var player = referee.PlayerTurn();
        if(ScoringPosition(other, player) & referee.InPlay()){
            Debug.Log("ball in endzone");
            StopCoroutine(waitRoutine);
        }
        if(ScoringPosition(other, player) & referee.Scoreable()){
            Debug.Log("touchdown, stopping play");
            referee.SetPlayState(Referee.PlayState.Stopped);
            referee.Touchdown(player);
        }
    }

    IEnumerator PlayEndAndWait(float seconds){
        yield return new WaitUntil(footballBody.IsSleeping);
        yield return new WaitForSeconds(seconds);
        Debug.Log("changing turns");
        referee.ChangeTurn();
        Debug.Log("playing");
        referee.SetPlayState(Referee.PlayState.Playing);
    }
}
