using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballMovement : MonoBehaviour
{
    Rigidbody footballBody;
    [SerializeField] Referee referee;
    [SerializeField] MoveCalculator moveCalculator;
    [SerializeField] GameObject player1Zone, player2Zone;
    float flickDisplacement = 0.45f;
    public float powerSpeed = 1f;
    float force = 0f;
    Vector3 player1KickoffPosition, player2KickoffPosition;
    Quaternion kickoffRotation;
    Vector3 cornerOffset;
    public bool scoringPosition;

    public void SetupKickoff(Referee.Player player)
    {
        if (player == Referee.Player.Player1)
        {
            transform.position = player1KickoffPosition;
            transform.rotation = kickoffRotation;
        }
        else
        {
            transform.position = player2KickoffPosition;
            transform.rotation = kickoffRotation * Quaternion.Euler(0, 0, 180);
        }
        footballBody.Sleep();
        referee.ReadyForKickoff();
    }

    void Kick(Referee.Player player)
    {
        var position = Vector3.zero;
        var direction = Vector3.zero;
        switch (player)
        {
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
                force = Random.Range(0.5f, 0.95f);
                break;
        }
        footballBody.AddForceAtPosition(
                force * direction.normalized,
                position,
                ForceMode.Impulse
            );
        force = 0f;
    }

    void Drive(Referee.Player player)
    {
        var direction = Vector3.zero;
        switch (player)
        {
            case Referee.Player.Player1:
                direction = Vector3.forward;
                break;
            case Referee.Player.Player2:
                direction = Vector3.back;
                break;
            case Referee.Player.Computer:
                direction = Vector3.back;
                var target = new Vector3(transform.position.x, 0, player1Zone.transform.position.z);
                var upperLimit = 1.2f + 0.15f * Mathf.Abs((player1Zone.transform.position.z - transform.position.z) /
                (player2Zone.transform.position.z - player1Zone.transform.position.z));
                force = Random.Range(0.6f, upperLimit) * moveCalculator.Force(target);
                break;
        }
        footballBody.AddForce(force * direction.normalized, ForceMode.Impulse);
        force = 0f;
    }

    void Play(Referee.Player player)
    {
        if (referee.playState == Referee.PlayState.Kickoff)
        {
            Kick(player);
        }
        else if (referee.playState == Referee.PlayState.Drive)
        {
            Drive(player);
        }
        StartCoroutine(referee.WaitAndUpdateState());
    }

    void Awake()
    {
        footballBody = GetComponent<Rigidbody>();
        player1KickoffPosition = new Vector3(
            player1Zone.transform.position.x,
            transform.position.y,
            player1Zone.transform.position.z
        );
        player2KickoffPosition = new Vector3(
            player2Zone.transform.position.x,
            transform.position.y,
            player2Zone.transform.position.z
        );
        kickoffRotation = transform.rotation;
        cornerOffset = transform.position + flickDisplacement * (transform.up.normalized);
        transform.position = player1KickoffPosition;
    }

    void Update()
    {
        if (Input.GetButton("Jump") & (referee.Playable(Referee.Player.Player1) | referee.Playable(Referee.Player.Player2)))
        {
            force += powerSpeed * Time.deltaTime;
        }
        else if (Input.GetButtonUp("Jump") & (referee.Playable(Referee.Player.Player1) | referee.Playable(Referee.Player.Player2)))
        {
            if (referee.Playable(Referee.Player.Player1))
            {
                Play(Referee.Player.Player1);
            }
            else
            {
                Play(Referee.Player.Player2);
            }
        }

        if (referee.Playable(Referee.Player.Computer))
        {
            Play(Referee.Player.Computer);
        }
    }

    bool ScoringPosition(Collider other, Referee.Player player)
    {
        return (other.CompareTag("PlayerEnd") & player != Referee.Player.Player1) |
               (other.CompareTag("OppoEnd") & player == Referee.Player.Player1);
    }

    void OnTriggerStay(Collider other)
    {
        if (ScoringPosition(other, referee.PlayerTurn()) & !scoringPosition)
        {
            scoringPosition = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerEnd") | other.CompareTag("OppoEnd"))
        {
            scoringPosition = false;
        }
    }
}
