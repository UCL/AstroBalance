using UnityEngine;

public class LockedOn : MonoBehaviour
{
    private string gazeCrosshairName = "GazeCrosshair";
    private string poseCrosshairName = "PoseCrosshair";

    private SpriteRenderer sprite;
    private Color defaultColor = Color.white;
    private Color singleLockColor = Color.red;
    private Color doubleLockColor = Color.cyan;

    private LockStatus lockStatus = LockStatus.None;
    private enum LockStatus
    {
        None,
        Single,
        Double
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = defaultColor;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == gazeCrosshairName && lockStatus == LockStatus.None)
        {
            sprite.color = singleLockColor;
            lockStatus = LockStatus.Single;
        }
        else if (other.gameObject.name == poseCrosshairName && lockStatus == LockStatus.Single)
        {
            sprite.color = doubleLockColor;
            lockStatus = LockStatus.Double;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == gazeCrosshairName && lockStatus != LockStatus.None)
        {
            sprite.color = defaultColor;
            lockStatus = LockStatus.None;
        } else if (other.gameObject.name == poseCrosshairName && lockStatus == LockStatus.Double)
        {
            sprite.color = singleLockColor;
            lockStatus = LockStatus.Single;
        }
    }
}
