using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    [SerializeField, Tooltip("Badge for the rocket launch game")]
    private Badge rocketBadge;

    [SerializeField, Tooltip("Badge for the star collector game.")]
    private Badge starCollectorBadge;

    [SerializeField, Tooltip("Badge for the star seek game.")]
    private Badge starSeekBadge;

    [SerializeField, Tooltip("Badge for the star map game.")]
    private Badge starMapBadge;

    [SerializeField, Tooltip("Badge for the space walking game")]
    private Badge spaceWalkBadge;

    [SerializeField, Tooltip("Badge for the zero gravity game")]
    private Badge zeroGravityBadge;

    void Awake()
    {
        SummaryData summary = CaptureSessionData.SummaryOfAllSessions();
        rocketBadge.SetCompleteGames(summary.nCompleteRocketLaunchGames);
        starCollectorBadge.SetCompleteGames(summary.nCompleteStarCollectorGames);
        starSeekBadge.SetCompleteGames(summary.nCompleteStarSeekGames);
        starMapBadge.SetCompleteGames(summary.nCompleteStarMapGames);
        spaceWalkBadge.SetCompleteGames(summary.nCompleteSpaceWalkGames);
        zeroGravityBadge.SetCompleteGames(summary.nCompleteZeroGravityGames);
    }
}
