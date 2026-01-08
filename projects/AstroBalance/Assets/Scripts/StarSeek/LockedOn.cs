using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LockedOn : MonoBehaviour
{
    [SerializeField, Tooltip("Number of seconds required to collect star")]
    private int doubleLockTime = 2;
    [SerializeField, Tooltip("Particle system to be shown on star collection")]
    private GameObject collectEffect;
    [SerializeField, Tooltip("Particle system to be shown on star double lock")]
    private GameObject lockedEffect;
    [SerializeField, Tooltip("Bloom intensity for single lock")]
    private float singleLockBloom = 10f;
    [SerializeField, Tooltip("Bloom intensity for double lock")]
    private float doubleLockBloom = 20f;

    private string gazeCrosshairName = "GazeCrosshair";
    private string poseCrosshairName = "PoseCrosshair";

    private SpriteRenderer sprite;
    private Bloom bloom;
    private Color defaultColor = Color.white;
    private GameObject doubleLockSparkle;

    private LockStatus lockStatus = LockStatus.None;
    private float doubleLockStart;
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

        Volume globalVolume = FindFirstObjectByType<Volume>();
        globalVolume.profile.TryGet(out bloom);
        bloom.intensity.value = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (lockStatus == LockStatus.Double && (Time.time - doubleLockStart >= doubleLockTime))
        {
            // collect star
            Destroy(doubleLockSparkle);
            GameObject collectedSparkle = Instantiate<GameObject>(collectEffect);
            collectedSparkle.transform.position = transform.position;
            Destroy(collectedSparkle, 1.0f);
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == gazeCrosshairName && lockStatus == LockStatus.None)
        {
            lockStatus = LockStatus.Single;
            bloom.intensity.value = singleLockBloom;
        }
        else if (other.gameObject.name == poseCrosshairName && lockStatus == LockStatus.Single)
        {
            lockStatus = LockStatus.Double;
            doubleLockStart = Time.time;
            bloom.intensity.value = doubleLockBloom;
            doubleLockSparkle = Instantiate<GameObject>(lockedEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == gazeCrosshairName && lockStatus != LockStatus.None)
        {
            lockStatus = LockStatus.None;
            bloom.intensity.value = 0f;
        } else if (other.gameObject.name == poseCrosshairName && lockStatus == LockStatus.Double)
        {
            lockStatus = LockStatus.Single;
            bloom.intensity.value = singleLockBloom;
            Destroy(doubleLockSparkle);
        }
    }
}
