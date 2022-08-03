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
    public int nPlayers = 1;
    public enum PlayState{
        Playing,
        Waiting,
        Stopped
    }
    private PlayState playState = PlayState.Playing;
        public enum Player{
        Player1,
        Player2,
        Computer,
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
        if (turn == Player.Player1){
            if(nPlayers == 2){
                turn = Player.Player2;
            } else {
                turn = Player.Computer;
            }
        } else {
            turn = Player.Player1;
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
        if (player == Player.Player1) {
            playerScore += 6;
        } else {
            opponentScore += 6;
        }
        SetScore();
        StartCoroutine(movement.SetupKickoff(player));
    }

    void SetScore()
    {
        scoreboard.text = "Player 1: " + playerScore.ToString() + "\nPlayer 2: " + opponentScore.ToString();
    }

    void Start(){
        footballBody = football.GetComponent<Rigidbody>();
    }
}
