using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    [SerializeField, Tooltip("Number of seconds to highlight a star for")]
    private int highlightTime = 1;
    [SerializeField, Tooltip("Minimum number of stars in a sequence")]
    private int minSequenceLength = 2;

    private List<StarMapStar> stars;
    private StarMapManager gameManager;
    private List<StarMapStar> currentSequence;

    private void Awake()
    {
        stars = new List<StarMapStar>(gameObject.GetComponentsInChildren<StarMapStar>());
        gameManager = FindFirstObjectByType<StarMapManager>();

        foreach (StarMapStar star in stars)
        {
            star.SetConstellation(this);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSequence = GenerateStarSequence(minSequenceLength);
        HighlightStarSequence(currentSequence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetNumberOfStars()
    {
        return stars.Count();
    }

    private void ResetStars()
    {
        foreach (StarMapStar star in stars)
        {
            star.ResetStar();
        }
    }

    private void EnableStarSelection()
    {
        foreach (StarMapStar star in stars)
        {
            star.EnableSelection();
        }
    }

    public void ShowNewSequence()
    {
        ResetStars();
        currentSequence = GenerateStarSequence(minSequenceLength);
        StartCoroutine(HighlightStarSequence(currentSequence));
    }

    public void AddGuess(StarMapStar star)
    {
        if (currentSequence[0] == star)
        {
            // guessed star is correct.
            // Remove it from the stars left to guess
            currentSequence.RemoveAt(0);

            if (currentSequence.Count() == 0)
            {
                // whole sequence has been guessed, create a new one
                ShowNewSequence();
            }

        } 
        else
        {
            // guess was wrong, create a new sequence
            ShowNewSequence();
        }
    }

    public List<StarMapStar> GenerateStarSequence(int length)
    {
        if (length > stars.Count())
        {
            length = stars.Count();
        }

        List<StarMapStar> starSequence = new List<StarMapStar>();
        List<StarMapStar> availableStars = new List<StarMapStar>(stars);

        for (int i = 0; i < length; i++)
        {
            // choose a random star, then remove it so there are no
            // repeats in the sequence
            StarMapStar randomStar = availableStars[Random.Range(0, availableStars.Count())];
            starSequence.Add(randomStar);
            availableStars.Remove(randomStar);
        }

        return starSequence;

    }

    IEnumerator HighlightStarSequence(List<StarMapStar> starSequence)
    {
        foreach (StarMapStar star in starSequence)
        {
            star.HighlightStar(highlightTime);
            yield return new WaitForSeconds(highlightTime);
        }

        EnableStarSelection();
    }
}
