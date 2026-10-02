using System.Collections.Generic;
using UnityEngine;

public class CorsiBlock : MonoBehaviour
{
    [SerializeField, Tooltip("Game object to represent stars in the constellation")]
    private GameObject StarObject;

    [SerializeField, Tooltip("X-size of Corsi Block")]
    private const int BlockXDim = 320;

    [SerializeField, Tooltip("Y-size of Corsi Block")]
    private const int BlockYDim = 250;

    [SerializeField, Tooltip("List of Star Positions in Corsi Block Units")]
    private List<Vector2> StarPositions;

    private List<GameObject> Stars;

    [SerializeField, Tooltip("Configurable constellation prefab object.")]
    private GameObject ConstellationObj;

    private GameObject myConstellation;

    [SerializeField, Tooltip("Percentage of the screen height taken up by constellation")]
    private float FractionalHeight = 0.8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    public Constellation ConstructConstellation()
    {
        // Use the list of positions to construct a constellation and scale it correctly
        myConstellation = Instantiate<GameObject>(ConstellationObj);
        float CameraHeight = Camera.main.orthographicSize;
        float ConstellationHalfHeight = CameraHeight * FractionalHeight;
        float ScaleFactor = (ConstellationHalfHeight * 2) / BlockYDim;
        float ConstellationHalfWidth = ScaleFactor * BlockXDim * 0.5f;
        Stars = new List<GameObject>();
        List<StarMapStar> StarScriptObjs = new List<StarMapStar>();
        foreach (Vector2 p in StarPositions)
        {
            Vector2 pos =
                p * ScaleFactor - new Vector2(ConstellationHalfWidth, ConstellationHalfHeight);
            GameObject star = Instantiate<GameObject>(StarObject);
            star.transform.position = new Vector3(pos.x, pos.y, 0f);
            Stars.Add(star);
            StarScriptObjs.Add(star.GetComponent<StarMapStar>());
        }
        myConstellation.GetComponent<Constellation>().InitStars(StarScriptObjs);

        return myConstellation.GetComponent<Constellation>();
    }

    // Update is called once per frame
    void Update() { }
}
