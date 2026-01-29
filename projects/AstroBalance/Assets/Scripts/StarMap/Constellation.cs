using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StarMapManager;

public class Constellation : MonoBehaviour
{
    [SerializeField, Tooltip("Minimum number of stars in a sequence")]
    private int minSequenceLength = 2;

    [SerializeField, Tooltip("Number of incorrect sequences before reducing length")]
    private int maxIncorrectSequences = 2;

    [
        SerializeField,
        Tooltip("Number of seconds to highlight each star when showing a new sequence")
    ]
    private int showSequenceHighlight = 1;

    [SerializeField, Tooltip("Number of seconds to delay before showing a new star sequence")]
    private float showSequenceDelay = 1f;

    [SerializeField, Tooltip("Number of seconds to highlight a correct sequence")]
    private float correctSequenceHighlight = 1.5f;

    [SerializeField, Tooltip("Number of seconds to highlight an incorrect sequence")]
    private float incorrectSequenceHighlight = 1f;

    [
        SerializeField,
        Tooltip("Number of seconds to delay before highlighting a correct/incorrect sequence")
    ]
    private float completeSequenceDelay = 0.5f;

    private List<StarMapStar> stars;
    private StarMapManager gameManager;
    private List<StarMapStar> currentSequence; // stars left to guess
    private List<StarMapStar> selectedStars; // stars selected so far
    private int currentSequenceLength;
    private int incorrectSequences = 0; // Incorrect sequences at current length
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
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public int GetNumberOfStars()
    {
        // Populate list of stars, if Awake() hasn't been called yet.
        // When we're choosing a constellation to spawn, we need to know how many stars it
        // contains before we instantiate it (i.e. before Awake is called)
        if (stars == null || stars.Count() == 0)
        {
            stars = new List<StarMapStar>(gameObject.GetComponentsInChildren<StarMapStar>());
        }

        return stars.Count();
    }

    /// <summary>
    /// Choose a new random sequence of stars, and display it to the player.
    /// Once display completes, stars are enabled for selection.
    /// </summary>
    /// <param name="repeatOrder">Order the player must repeat the sequence in</param>
    public void ShowNewSequence(RepeatOrder repeatOrder)
    {
        order = repeatOrder;
        currentSequence = new List<StarMapStar>();
        selectedStars = new List<StarMapStar>();

        ResetStars();
        currentSequence = GenerateStarSequence(currentSequenceLength);
        StartCoroutine(HighlightNewStarSequence(currentSequence));
    }

    /// <summary>
    /// Add a guess for the next star in the sequence.
    /// </summary>
    /// <param name="star">The selected star.</param>
    public void AddGuess(StarMapStar star)
    {
        selectedStars.Add(star);

        if (currentSequence[0] == star)
        {
            HandleCorrectGuess();
        }
        else
        {
            HandleIncorrectGuess();
        }
    }

    private void ResetStars()
    {
        foreach (StarMapStar star in stars)
        {
            star.ResetStar();
        }
    }

    private void DisableStarSelection()
    {
        foreach (StarMapStar star in stars)
        {
            star.DisableSelection();
        }
    }

    private void EnableStarSelection()
    {
        foreach (StarMapStar star in stars)
        {
            star.EnableSelection();
        }
    }

    private IEnumerator CompleteSequenceAndTriggerNext(bool correctGuess)
    {
        DisableStarSelection();

        // wait for n seconds before highlighting the completed sequence,
        // makes it easier for the player to see the start
        yield return new WaitForSeconds(completeSequenceDelay);

        // highlight completed sequence, in different ways
        // depending on correct vs incorrect guess
        float highlightTime;
        if (correctGuess)
        {
            highlightTime = correctSequenceHighlight;
        }
        else
        {
            highlightTime = incorrectSequenceHighlight;
        }

        foreach (StarMapStar star in selectedStars)
        {
            if (correctGuess)
            {
                star.HighlightCorrectForSeconds(highlightTime);
            }
            else
            {
                star.HighlightIncorrectForSeconds(highlightTime);
            }
        }
        yield return new WaitForSeconds(highlightTime);

        ShowNewSequence(order);
    }

    private void HandleCorrectGuess()
    {
        // Remove star from the stars left to guess
        currentSequence.RemoveAt(0);
        if (currentSequence.Count != 0)
            return;

        // whole sequence has been guessed correctly
        gameManager.UpdateScore(currentSequenceLength);

        if (gameManager.IsGameActive() && currentSequenceLength < GetNumberOfStars())
        {
            currentSequenceLength += 1;
            incorrectSequences = 0;
            StartCoroutine(CompleteSequenceAndTriggerNext(true));
        }
        else
        {
            ResetStars();
        }
    }

    private void HandleIncorrectGuess()
    {
        incorrectSequences += 1;

        // reduce length of next sequence, if the player
        // has had n incorrect guesses in a row
        if (
            incorrectSequences == maxIncorrectSequences
            && currentSequenceLength > minSequenceLength
        )
        {
            currentSequenceLength -= 1;
            incorrectSequences = 0;
        }

        StartCoroutine(CompleteSequenceAndTriggerNext(false));
    }

    /// <summary>
    /// Create a random sequence of stars of the given length.
    /// </summary>
    /// <param name="length">Number of stars in sequence.</param>
    /// <returns>A list of stars</returns>
    private List<StarMapStar> GenerateStarSequence(int length)
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

    private IEnumerator HighlightNewStarSequence(List<StarMapStar> starSequence)
    {
        // wait for n seconds before highlighting the sequence, makes
        // it easier for the player to see the start
        yield return new WaitForSeconds(showSequenceDelay);

        foreach (StarMapStar star in starSequence)
        {
            star.HighlightCorrectForSeconds(showSequenceHighlight);
            yield return new WaitForSeconds(showSequenceHighlight);
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
