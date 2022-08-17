using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Referee : MonoBehaviour
{
    [SerializeField] GameObject football, footballFG, player1FG, player2FG, gameOverPanel;
    [SerializeField] FootballMovement movement;
    [SerializeField] FieldGoalFlick movementFG;
    Rigidbody footballBody;
    [SerializeField] TextMeshProUGUI scoreboard, gameClock;
    [SerializeField] int gameTime;
    public enum PlayState { Kickoff, Waiting, Drive, FGAttempt };
    public PlayState playState;
    public enum Player { Player1, Player2, Computer };
    Player turn = Player.Player1;
    int player1Score = 0, player2Score = 0;

    public void ChangeTurn()
    {
        if (turn == Player.Player1)
        {
            if (DataManager.Instance.gameParameters.nPlayers == 2)
            {
                turn = Player.Player2;
            }
            else
            {
                turn = Player.Computer;
            }
        }
        else
        {
            turn = Player.Player1;
        }
    }

    public Player PlayerTurn()
    {
        return turn;
    }

    public bool Playable(Player player)
    {
        return player == turn & (playState == PlayState.Kickoff | playState == PlayState.Drive);
    }

    bool IsPlayFinished()
    {
        return footballBody.IsSleeping() | IsOutOfBounds();
    }

    float DriveWaitTime()
    {
        if (turn == Player.Computer)
        {
            return 0.5f;
        }
        return 0;
    }

    void ActivatePrimaryPlay()
    {
        player1FG.SetActive(false);
        player2FG.SetActive(false);
        footballFG.SetActive(false);
        footballBody = football.GetComponent<Rigidbody>();
        football.SetActive(true);
    }

    void ActivateFGPlay()
    {
        football.SetActive(false);
        footballFG.SetActive(true);
        footballBody = footballFG.GetComponent<Rigidbody>();
        if (turn == Referee.Player.Player1)
        {
            player2FG.SetActive(true);
        }
        else
        {
            player1FG.SetActive(true);
        }
        gameObject.SetActive(true);
        footballBody.Sleep();
    }

    public IEnumerator WaitAndUpdateState()
    {
        playState = PlayState.Waiting;
        yield return new WaitUntil(IsPlayFinished);
        if (IsOutOfBounds())
        {
            yield return new WaitForSeconds(0.5f);
            movement.SetupKickoff(turn);
            yield return new WaitForSeconds(DriveWaitTime());
            playState = PlayState.Kickoff;
            yield break;
        }
        if (movement.scoringPosition & football.activeInHierarchy)
        {
            Touchdown(turn);
            yield return new WaitForSeconds(0.5f);
            ActivateFGPlay();
            // the following line causes an indefinite wait?
            //yield return new WaitForSeconds(DriveWaitTime());
            playState = PlayState.FGAttempt;
            yield break;
        }
        if (movementFG.scoringPosition & footballFG.activeInHierarchy)
        {
            ExtraPoint(turn);
            yield return new WaitForSeconds(0.5f);
            ActivatePrimaryPlay();
            movement.SetupKickoff(turn);
            yield break;
        }
        ChangeTurn();
        yield return new WaitForSeconds(DriveWaitTime());
        playState = PlayState.Drive;
    }

    public bool IsOutOfBounds()
    {
        return footballBody.transform.position.y < 0f;
    }

    public void Touchdown(Player player)
    {
        if (player == Player.Player1)
        {
            player1Score += 6;
        }
        else
        {
            player2Score += 6;
        }
        SetScore();
    }

    public void ExtraPoint(Player player)
    {
        if (player == Player.Player1)
        {
            player1Score += 1;
        }
        else
        {
            player2Score += 1;
        }
        SetScore();
    }

    bool IsFinalScore()
    {
        return (
            DataManager.Instance.gameParameters.gameMode == DataManager.GameMode.Points & (
                player1Score >= DataManager.Instance.gameParameters.pointLimit |
                player2Score >= DataManager.Instance.gameParameters.pointLimit
            )
        );
    }

    void SetScore()
    {
        // Check for end of game if points game mode
        if (IsFinalScore())
        {
            GameOver();
        }
        scoreboard.text = $"{DataManager.Instance.gameParameters.player1Name}: {player1Score}\n{DataManager.Instance.gameParameters.player2Name}: {player2Score}";
    }

    void SetTime()
    {
        int minutes = gameTime / 60;
        int remainderSeconds = gameTime % 60;
        if (remainderSeconds >= 10)
        {
            gameClock.text = $"Time: {minutes}:{remainderSeconds}";
        }
        else
        {
            gameClock.text = $"Time: {minutes}:0{remainderSeconds}";
        }
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        StopAllCoroutines();
        Time.timeScale = 0;
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    IEnumerator CountTime()
    {
        yield return new WaitForSeconds(1);
        gameTime--;
        SetTime();
        if (gameTime <= 0)
        {
            GameOver();
        }
        StartCoroutine(CountTime());
    }

    void Awake()
    {
        if (DataManager.Instance == null)
        {
            DataManager.Instance = new GameObject("DataManager").AddComponent<DataManager>();
        }
        if (DataManager.Instance.gameParameters.gameMode == DataManager.GameMode.Time)
        {
            gameTime = DataManager.Instance.gameParameters.timeLimit;
        }
        footballBody = football.GetComponent<Rigidbody>();
    }

    void Start()
    {
        StopAllCoroutines();
        SetScore();
        if (DataManager.Instance.gameParameters.gameMode == DataManager.GameMode.Time)
        {
            SetTime();
            gameClock.gameObject.SetActive(true);
            StartCoroutine(CountTime());
        }
        movement.SetupKickoff(turn);
    }
}
