using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using static StarMapManager;

public class Constellation : MonoBehaviour
{
    [SerializeField, Tooltip("Number of seconds to highlight a star for")]
    private int highlightTime = 1;
    [SerializeField, Tooltip("Minimum number of stars in a sequence")]
    private int minSequenceLength = 2;
    [SerializeField, Tooltip("Number of incorrect sequences before reducing length")]
    private int maxIncorrectSequences = 2;

    private List<StarMapStar> stars;
    private StarMapManager gameManager;
    private List<StarMapStar> currentSequence;
    private int currentSequenceLength;
    private int incorrectSequences = 0;  // Incorrect sequences at current length
    private RepeatOrder order = RepeatOrder.Same;

    private void Awake()
    {
        stars = new List<StarMapStar>(gameObject.GetComponentsInChildren<StarMapStar>());
        gameManager = FindFirstObjectByType<StarMapManager>();
        currentSequenceLength = minSequenceLength;

        foreach (StarMapStar star in stars)
        {
            star.SetConstellation(this);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public void ShowNewSequence(RepeatOrder repeatOrder)
    {
        order = repeatOrder;

        ResetStars();
        currentSequence = GenerateStarSequence(currentSequenceLength);
        StartCoroutine(HighlightStarSequence(currentSequence));
    }

    public void AddGuess(StarMapStar star)
    {
        if (currentSequence[0] == star)
        {
            HandleCorrectGuess();
        } 
        else
        {
            HandleIncorrectGuess();
        }
    }

    private void HandleCorrectGuess()
    {
        // Remove star from the stars left to guess
        currentSequence.RemoveAt(0);
        incorrectSequences = 0;

        if (currentSequence.Count() == 0)
        {
            // whole sequence has been guessed correctly
            gameManager.UpdateScore();

            if (gameManager.IsGameActive())
            {
                currentSequenceLength += 1;
                ShowNewSequence(order);
            }
            else
            {
                ResetStars();
            }
        }
    }

    private void HandleIncorrectGuess()
    {
        incorrectSequences += 1;

        // reduce length of next sequence, if the player 
        // has had n incorrect guesses in a row
        if (incorrectSequences == maxIncorrectSequences && currentSequenceLength > minSequenceLength)
        {
            currentSequenceLength -= 1;
            incorrectSequences = 0;
        }
        ShowNewSequence(order);

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

    private IEnumerator HighlightStarSequence(List<StarMapStar> starSequence)
    {
        foreach (StarMapStar star in starSequence)
        {
            star.HighlightStar(highlightTime);
            yield return new WaitForSeconds(highlightTime);
        }

        // When the repeat order is opposite, reverse the order of the
        // sequence for guessing
        if (order == RepeatOrder.Opposite)
        {
            currentSequence.Reverse();
        }

        EnableStarSelection();
    }
}
