using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Referee : MonoBehaviour
{
    public GameObject football, footballFG, playerFG, oppoFG;
    [SerializeField] GameObject gameOverPanel;
    public FootballMovement movement;
    public CameraFollow cameraFollow;
    private Rigidbody footballBody;
    [SerializeField] TextMeshProUGUI scoreboard, gameClock;
    public int nPlayers = 1;
    public string player1Name, player2Name;
    [SerializeField] DataManager.GameMode gameMode;
    [SerializeField] int gameTime, gamePoints;
    public enum PlayState{Playing, Waiting, Stopped};
    private PlayState playState = PlayState.Playing;
    public enum Player{Player1, Player2, Computer};
    private Player turn = Player.Player1;
    private int player1Score, player2Score = 0;
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
            player1Score += 6;
        } else {
            player2Score += 6;
        }
        SetScore();
        StartCoroutine(movement.SetupFG(player));
    }

    public void ExtraPoint(Player player){
        if (player == Player.Player1) {
            player1Score += 1;
        } else {
            player2Score += 1;
        }
        SetScore();
    }

    public IEnumerator ResetPlay(){
        StartCoroutine(movement.SetupKickoff(turn));
        yield return new WaitForSeconds(1);
        playerFG.SetActive(false);
        oppoFG.SetActive(false);
        footballFG.SetActive(false);
        cameraFollow.Switch();
        football.SetActive(true);
    }

    void SetScore()
    {
        // Check for end of game if points game mode
        if (gameMode == DataManager.GameMode.Points) {
            if (player1Score >= gamePoints | player2Score >= gamePoints) {
                GameOver();
            }
        }
        scoreboard.text = $"{player1Name}: {player1Score}\n{player2Name}: {player2Score}";
    }

    void SetTime() {
        int minutes = gameTime / 60;
        int remainderSeconds = gameTime % 60;
        if (remainderSeconds >= 10) {
            gameClock.text = $"Time: {minutes}:{remainderSeconds}";
        } else {
            gameClock.text = $"Time: {minutes}:0{remainderSeconds}";
        }
    }

    void GameOver() {
        playState = PlayState.Stopped;
        gameOverPanel.SetActive(true);
        StopAllCoroutines();
        Time.timeScale = 0;
    }

    IEnumerator CountTime() {
        yield return new WaitForSeconds(1);
        gameTime--;
        SetTime();
        if (gameTime <= 0) {
            GameOver();
        }
        StartCoroutine(CountTime());
    }

    void Start(){
        if (DataManager.Instance != null) {
            nPlayers = DataManager.Instance.nPlayers;
            player1Name = DataManager.Instance.player1Name;
            if (nPlayers > 1) {
                player2Name = DataManager.Instance.player2Name;
            } else {
                player2Name = "Computer";
            }
            gameMode = DataManager.Instance.gameMode;
            gamePoints = DataManager.Instance.gamePoints;
            gameTime = DataManager.Instance.gameTime;
        }
        footballBody = football.GetComponent<Rigidbody>();
        SetScore();
        if (gameMode == DataManager.GameMode.Time) {
            SetTime();
            gameClock.gameObject.SetActive(true);
            StartCoroutine(CountTime());
        }
    }
}
