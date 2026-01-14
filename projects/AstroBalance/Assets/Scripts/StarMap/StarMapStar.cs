using System.Collections;
using UnityEngine;

public class StarMapStar : MonoBehaviour
{
    [SerializeField, Tooltip("Number of seconds to select a star")]
    private float selectionTime = 0.5f;

    private Color highlightColor = Color.red;
    private Color defaultColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private StarMapManager gameManager;
    private StarStatus starStatus;

    private float selectionStartTime;

    private enum StarStatus
    {
        None,
        SelectionPending,
        Selected
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindFirstObjectByType<StarMapManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (starStatus == StarStatus.SelectionPending && (Time.time - selectionStartTime > selectionTime))
        {
            SetStarStatus(StarStatus.Selected);
        }
    }

    /// <summary>
    /// Highlight star for the given number of seconds.
    /// </summary>
    /// <param name="seconds">Number of seconds.</param>
    public void HighlightStar(int seconds)
    {
        spriteRenderer.color = highlightColor;
        StartCoroutine(ResetColor(seconds));
    }

    IEnumerator ResetColor(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        spriteRenderer.color = defaultColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When we are showing a sequence, we don't want stars to respond to 
        // the player's gaze
        if (gameManager.GetGamePhase() == StarMapManager.GamePhase.ShowSequence)
        {
            return;
        }

        if (starStatus != StarStatus.Selected)
        {
            SetStarStatus(StarStatus.SelectionPending);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // When we are showing a sequence, we don't want stars to respond to 
        // the player's gaze
        if (gameManager.GetGamePhase() == StarMapManager.GamePhase.ShowSequence)
        {
            return;
        }

        if (starStatus != StarStatus.Selected)
        {
            SetStarStatus(StarStatus.None);
        }
    }

    private void SetStarStatus(StarStatus status) {
        if (status == StarStatus.None)
        {
            spriteRenderer.color = defaultColor;
        }
        else if (status == StarStatus.SelectionPending)
        {
            spriteRenderer.color = highlightColor;
            selectionStartTime = Time.time;
        } else
        {
            spriteRenderer.color = Color.blue;
        }

        starStatus = status;
    }
}
