using TMPro;
using UnityEngine;

public class PoseAvatar : MonoBehaviour
{
    [SerializeField, Tooltip("List of pose sprites (in order of appearance in game)")]
    private Sprite[] sprites;

    [SerializeField, Tooltip("List of pose explanation text (in order of appearance in game)")]
    private string[] poseExplanations;

    [SerializeField, Tooltip("Text mesh pro object for pose explanation text")]
    private TextMeshProUGUI poseText;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = -1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Show the next pose sprite + explanation text.
    /// </summary>
    /// <returns>true, if next sprite was successfully shown.
    /// false, if there are no more sprites left.</returns>
    public bool ShowNextSprite()
    {
        currentIndex++;
        if (currentIndex >= sprites.Length)
        {
            return false;
        }

        spriteRenderer.sprite = sprites[currentIndex];
        poseText.text = poseExplanations[currentIndex];
        poseText.gameObject.SetActive(true);

        return true;
    }

    public void HideExplanationText()
    {
        poseText.gameObject.SetActive(false);
    }
}
