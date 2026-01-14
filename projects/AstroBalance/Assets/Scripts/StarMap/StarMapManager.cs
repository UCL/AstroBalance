using System.Collections.Generic;
using UnityEngine;

public class StarMapManager : MonoBehaviour
{

    private int score = 0;
    private GamePhase gamePhase = GamePhase.ShowSequence;
    private List<int> currentSequence;

    public enum GamePhase
    {
        ShowSequence,
        GuessSequence
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GamePhase GetGamePhase()
    {
        return gamePhase;
    }

    public void enterGuessPhase()
    {
        gamePhase = GamePhase.GuessSequence;
    }
}
