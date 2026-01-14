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
    private List<int> currentSequence;

    private void Awake()
    {
        stars = new List<StarMapStar>(gameObject.GetComponentsInChildren<StarMapStar>());
        gameManager = FindFirstObjectByType<StarMapManager>();
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

    public List<int> GenerateStarSequence(int length)
    {
        if (length > stars.Count())
        {
            length = stars.Count();
        }

        List<int> randomSequence = new List<int>();
        List<int> indexes = Enumerable.Range(0, stars.Count()).ToList();

        for (int i = 0; i < length; i++)
        {
            // choose a random item, then remove it so there are no
            // repeats in the sequence
            int randomIndex = indexes[Random.Range(0, indexes.Count())];
            randomSequence.Add(randomIndex);
            indexes.Remove(randomIndex);
        }

        return randomSequence;

    }

    IEnumerator HighlightStarSequence(List<int> starIndexes)
    {
        foreach (int index in starIndexes)
        {
            StarMapStar star = stars[index];
            star.HighlightStar(highlightTime);
            yield return new WaitForSeconds(highlightTime);
        }

        EnableStarSelection();
    }
}
