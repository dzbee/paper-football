using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Referee : MonoBehaviour
{
    public GameObject football;
    public FootballMovement movement;
    private Rigidbody footballBody;
    public TextMeshProUGUI scoreboard;
    public enum PlayState{
        Playing,
        Waiting,
        Stopped
    }
    private PlayState playState = PlayState.Playing;
        public enum Player{
        Player1,
        Player2,
    }
    private Player turn = Player.Player1;
    private int playerScore, opponentScore = 0;
    private bool kickoff = true;
    
    public void SetPlayState(PlayState state) {
        playState = state;
    }

    public bool InPlay(){
        return playState != PlayState.Stopped;
    }

    public void ChangeTurn(){
        switch (turn) {
            case Player.Player1:
                turn = Player.Player2;
                break;
            case Player.Player2:
                turn = Player.Player1;
                break;
        }
    }

    public Player PlayerTurn(){
        return turn;
    }

    public bool Pushable(Player player){
        return !kickoff & turn == player & footballBody.IsSleeping();
    }

    public bool Kickable(Player player){
        return kickoff & turn == player & footballBody.IsSleeping();
    }

    public bool Playable(Player player){
        return playState == PlayState.Playing & (Pushable(player) | Kickable(player));
    }

    public void ReadyForKickoff(bool ready){
        kickoff = ready;
    }
    
    public bool OutOfBounds(){
        return football.transform.position.y < 0f;
    }
    
    public bool Scoreable(){
        return footballBody.IsSleeping() & InPlay();
    }

    public void Touchdown(Player player){
        switch (player) {
            case Player.Player1:
                playerScore += 6;
                break;
            case Player.Player2:
                opponentScore += 6;
                break;
        }
        SetScore();
        StartCoroutine(movement.SetupKickoff(player));
    }

    void SetScore()
    {
        scoreboard.text = "Player: " + playerScore.ToString() + "\nOpponent: " + opponentScore.ToString();
    }

    void Start(){
        footballBody = football.GetComponent<Rigidbody>();
    }
}
