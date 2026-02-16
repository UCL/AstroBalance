using System.Collections;
using UnityEngine;

public class StarMapStar : MonoBehaviour
{
    [SerializeField, Tooltip("Particle system to highlight star")]
    private GameObject sparkleEffect;

    [SerializeField, Tooltip("Size increase for correctly selected star")]
    private float sizeIncrease = 1.3f;

    [SerializeField, Tooltip("Size decrease for incorrectly selected star")]
    private float sizeDecrease = 0.5f;

    [SerializeField, Tooltip("Color for correctly selected star")]
    private Color correctColor = new Color(1, 0.53f, 0.47f);

    [SerializeField, Tooltip("Color for incorrectly selected star")]
    private Color incorrectColor = Color.red;

    private Color defaultColor = Color.white;
    private SpriteRenderer spriteRenderer;
    private Collider2D starCollider;
    private Constellation constellation;
    private SelectionStatus selectionStatus;
    private GameObject starSparkle;

    private Vector3 defaultScale;
    private bool selectionEnabled = false;

    private enum SelectionStatus
    {
        None,
        Selected,
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        starCollider = GetComponent<Collider2D>();
        defaultScale = transform.localScale;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (!selectionEnabled || selectionStatus == SelectionStatus.Selected)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == starCollider)
            {
                SetSelectionStatus(SelectionStatus.Selected);
            }
        }
    }

    /// <summary>
    /// When selected, the star will report a guess to this constellation.
    /// </summary>
    public void SetConstellation(Constellation inputConstellation)
    {
        constellation = inputConstellation;
    }

    /// <summary>
    /// Reset star colour / size, and disable selection.
    /// </summary>
    public void ResetStar()
    {
        DisableSelection();
        SetSelectionStatus(SelectionStatus.None);
    }

    public void DisableSelection()
    {
        selectionEnabled = false;
    }

    public void EnableSelection()
    {
        selectionEnabled = true;
    }

    /// <summary>
    /// Highlight correct star for the given number of seconds.
    /// </summary>
    /// <param name="seconds">Number of seconds.</param>
    public void HighlightCorrectForSeconds(float seconds)
    {
        spriteRenderer.color = correctColor;
        starSparkle = Instantiate<GameObject>(
            sparkleEffect,
            transform.position,
            Quaternion.identity
        );
        transform.localScale = defaultScale * sizeIncrease;

        StartCoroutine(StopHighlight(seconds));
    }

    /// <summary>
    /// Highlight incorrect star for the given number of seconds.
    /// </summary>
    /// <param name="seconds">Number of seconds.</param>
    public void HighlightIncorrectForSeconds(float seconds)
    {
        spriteRenderer.color = incorrectColor;
        transform.localScale = defaultScale * sizeDecrease;

        StartCoroutine(StopHighlight(seconds));
    }

    IEnumerator StopHighlight(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResetStar();
    }

    private void SetSelectionStatus(SelectionStatus status)
    {
        if (status == SelectionStatus.None)
        {
            spriteRenderer.color = defaultColor;
            Destroy(starSparkle);
            transform.localScale = defaultScale;
            selectionStatus = status;
        }
        else
        {
            // Star is selected
            spriteRenderer.color = correctColor;
            transform.localScale = defaultScale * sizeIncrease;
            selectionStatus = status;

            if (constellation != null)
            {
                constellation.AddGuess(this);
            }
        }
    }
}
