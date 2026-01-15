using System.Collections;
using UnityEngine;

public class StarMapStar : MonoBehaviour
{
    [SerializeField, Tooltip("Number of seconds to select a star")]
    private float selectionTime = 0.5f;

    private Color highlightColor = Color.red;
    private Color defaultColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private Constellation constellation;
    private SelectionStatus selectionStatus;

    private float selectionStartTime;
    private bool selectionEnabled = false;

    private enum SelectionStatus
    {
        None,
        SelectionPending,
        Selected
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionStatus == SelectionStatus.SelectionPending && (Time.time - selectionStartTime > selectionTime))
        {
            SetSelectionStatus(SelectionStatus.Selected);
        }
    }

    /// <summary>
    /// When selected, the star will report a guess to this constellation.
    /// </summary>
    public void SetConstellation(Constellation inputConstellation)
    {
        constellation = inputConstellation;
    }

    public void ResetStar()
    {
        selectionEnabled = false;
        SetSelectionStatus(SelectionStatus.None);
    }

    public void EnableSelection()
    {
        selectionEnabled = true;
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
        if (!selectionEnabled)
        {
            return;
        }

        if (selectionStatus != SelectionStatus.Selected)
        {
            SetSelectionStatus(SelectionStatus.SelectionPending);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!selectionEnabled)
        {
            return;
        }

        if (selectionStatus != SelectionStatus.Selected)
        {
            SetSelectionStatus(SelectionStatus.None);
        }
    }

    private void SetSelectionStatus(SelectionStatus status) {
        if (status == SelectionStatus.None)
        {
            spriteRenderer.color = defaultColor;
            selectionStatus = status;
        }
        else if (status == SelectionStatus.SelectionPending)
        {
            spriteRenderer.color = highlightColor;
            selectionStartTime = Time.time;
            selectionStatus = status;
        } 
        else
        {
            // Star is selected
            spriteRenderer.color = Color.blue;
            selectionStatus = status;

            if (constellation != null)
            {
                constellation.AddGuess(this);
            }
        }
    }
}
