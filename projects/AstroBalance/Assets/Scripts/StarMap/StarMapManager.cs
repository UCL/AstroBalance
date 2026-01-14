using System.Collections.Generic;
using UnityEngine;

public class StarMapManager : MonoBehaviour
{
    [SerializeField, Tooltip("The constellation of stars")]
    private Constellation constellation;

    private int score = 0;
    private GamePhase gamePhase = GamePhase.ShowSequence;
    
    public enum GamePhase
    {
        ShowSequence,
        GuessSequence
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        constellation.ShowNewSequence();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public GamePhase GetGamePhase()
    //{
    //    return gamePhase;
    //}

    //public void SetGamePhase(GamePhase phase)
    //{
    //    if (phase == GamePhase.ShowSequence)
    //    {
    //        constellation.ShowNewSequence();
    //    }

    //    gamePhase = phase;
    //}
}
