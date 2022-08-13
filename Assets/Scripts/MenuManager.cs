using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject player1NameField, player2NameField;
    [SerializeField] GameObject playerIncrementButton, playerDecrementButton;
    [SerializeField] GameObject gameTimeButton, gamePointButton;
    [SerializeField] GameObject gameTimeSelect, gamePointSelect;
    [SerializeField] TextMeshProUGUI playerSelectorText, gameModeText, gamePointText, gameTimeText;
    int pointLimitIncrement = 7, maxPointLimit = 70;
    int timeLimitIncrement = 30, maxTimeLimit = 3600;

    public void StartGame() {
        SceneManager.LoadScene("Game");
    }

    public void Set1Player() {
        DataManager.Instance.nPlayers = 1;
        playerSelectorText.text = "1 Player";
        playerIncrementButton.SetActive(true);
        playerDecrementButton.SetActive(false);
        player2NameField.SetActive(false);
        var player1NameFieldPos = player1NameField.transform.position;
        player1NameField.transform.position = player1NameFieldPos + 90 * Vector3.right;
    }

    public void Set2Player() {
        DataManager.Instance.nPlayers = 2;
        playerSelectorText.text = "2 Player";
        playerIncrementButton.SetActive(false);
        playerDecrementButton.SetActive(true);
        var player1NameFieldPos = player1NameField.transform.position;
        player1NameField.transform.position = player1NameFieldPos - 90 * Vector3.right;
        player2NameField.SetActive(true);
    }

    public void SetPlayer1Name(string name) {
        DataManager.Instance.player1Name = name;
    }

    public void SetPlayer2Name(string name) {
        DataManager.Instance.player2Name = name;
    }

    public void SetGameTimeMode() {
        DataManager.Instance.gameMode = DataManager.GameMode.Time;
        gameModeText.text = "Time Limit";
        gameTimeButton.SetActive(false);
        gamePointButton.SetActive(true);
        gameTimeSelect.SetActive(true);
        gamePointSelect.SetActive(false);
    }

    public void SetGamePointMode() {
        DataManager.Instance.gameMode = DataManager.GameMode.Points;
        gameModeText.text = "Point Limit";
        gameTimeButton.SetActive(true);
        gamePointButton.SetActive(false);
        gameTimeSelect.SetActive(false);
        gamePointSelect.SetActive(true);
    }

    public void IncrementPointLimit() {
        var newPointLimit = DataManager.Instance.gamePoints + pointLimitIncrement;
        if (newPointLimit <= maxPointLimit) {
            DataManager.Instance.gamePoints = newPointLimit;
            gamePointText.text = DataManager.Instance.gamePoints.ToString();
        }
    }

    public void DecrementPointLimit() {
        var newPointLimit = DataManager.Instance.gamePoints - pointLimitIncrement;
        if (newPointLimit > 0) {
            DataManager.Instance.gamePoints = newPointLimit;
            gamePointText.text = DataManager.Instance.gamePoints.ToString();
        }
    }

    public void IncrementTimeLimit() {
        var newTimeLimit = DataManager.Instance.gameTime + timeLimitIncrement;
        if (newTimeLimit <= maxTimeLimit) {
            DataManager.Instance.gameTime = newTimeLimit;
            gameTimeText.text = TimeToString(DataManager.Instance.gameTime);
        }
    }

    public void DecrementTimeLimit() {
        var newTimeLimit = DataManager.Instance.gameTime - timeLimitIncrement;
        if (newTimeLimit > 0) {
            DataManager.Instance.gameTime = newTimeLimit;
            gameTimeText.text = TimeToString(DataManager.Instance.gameTime);
        }
    }

    string TimeToString(int seconds) {
        int minutes = seconds / 60;
        int remainderSeconds = seconds % 60;
        if (remainderSeconds < 10) {
            return $"{minutes}:0{remainderSeconds}";
        }
        return $"{minutes}:{remainderSeconds}";
    }
}
