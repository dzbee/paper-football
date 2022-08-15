using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public enum GameMode{Points, Time};

    [System.Serializable]
    public class GameParameters {
        public int nPlayers;
        public string player1Name, player2Name;
        public GameMode gameMode;
        public int pointLimit, timeLimit;

        public static GameParameters GetDefaults() {
            return new GameParameters() {
                nPlayers = 1,
                player1Name = "Player",
                player2Name = "Computer",
                gameMode = GameMode.Points,
                pointLimit = 7
            };
        }
    }

    public static DataManager Instance;
    public GameParameters gameParameters;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public DataManager() {
        gameParameters = GameParameters.GetDefaults();
    }
}
