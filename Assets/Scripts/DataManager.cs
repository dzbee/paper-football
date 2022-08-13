using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public int nPlayers = 1;
    public string player1Name, player2Name;
    public enum GameMode{Points, Time};
    public GameMode gameMode = GameMode.Points;
    public int gamePoints = 7;
    public int gameTime = 5 * 60;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
