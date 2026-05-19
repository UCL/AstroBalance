using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Badge : MonoBehaviour
{
    [SerializeField, Tooltip("Badge image")]
    private Image badgeImage;

    [SerializeField, Tooltip("Game number text")]
    private TextMeshProUGUI gameNumberText;

    [SerializeField, Tooltip("Default badge sprite")]
    private Sprite defaultSprite;

    [SerializeField, Tooltip("Bronze badge sprite")]
    private Sprite bronzeSprite;

    [SerializeField, Tooltip("Silver badge sprite")]
    private Sprite silverSprite;

    [SerializeField, Tooltip("Gold badge sprite")]
    private Sprite goldSprite;

    [SerializeField, Tooltip("Number of complete games required for bronze")]
    private int nGamesBronze = 10;

    [SerializeField, Tooltip("Nubmer of complete games required for silver")]
    private int nGamesSilver = 20;

    [SerializeField, Tooltip("Number of complete games required for gold")]
    private int nGamesGold = 30;

    public void SetCompleteGames(int nGames)
    {
        if (nGames >= nGamesGold)
        {
            badgeImage.sprite = goldSprite;
        }
        else if (nGames >= nGamesSilver)
        {
            badgeImage.sprite = silverSprite;
        }
        else if (nGames >= nGamesBronze)
        {
            badgeImage.sprite = bronzeSprite;
        }
        else
        {
            badgeImage.sprite = defaultSprite;
        }

        gameNumberText.text = nGames.ToString();
    }
}
