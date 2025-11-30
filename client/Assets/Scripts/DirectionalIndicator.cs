using UnityEngine;

public class DirectionalIndicator : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Indicator Settings")]
    public float distanceInFront = 2f;
    public float verticalOffset = -0.2f;
    public float hideDistance = 3f;

    private Transform cam;
    private SpriteRenderer sprite;

    void Start()
    {
        cam = Camera.main.transform;

        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite == null)
        {
            Debug.LogError("[DirectionalIndicator] Missing SpriteRenderer child!");
            enabled = false;
            return;
        }

        // Parent to camera
        transform.SetParent(cam, false);

        // Initial position
        transform.localPosition = new Vector3(0, verticalOffset, distanceInFront);
    }

    void Update()
    {
        if (cam == null || sprite == null)
            return;

        if (target == null)
        {
            sprite.enabled = false;
            return;
        }

        // Hide when close
        float dist = Vector3.Distance(cam.position, target.position);
        sprite.enabled = dist > hideDistance;

        // Keep in front of camera
        transform.localPosition = new Vector3(0, verticalOffset, distanceInFront);

        // Direction toward target (XZ plane)
        Vector3 toTarget = target.position - cam.position;
        toTarget.y = 0;

        if (toTarget.sqrMagnitude < 0.01f)
            return;

        toTarget.Normalize();

        Vector3 forward = cam.forward;
        forward.y = 0;
        forward.Normalize();

        float angle = Vector3.SignedAngle(forward, toTarget, Vector3.up);

        // Sprite points UP → rotate around Z
        transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    public void SetTarget(Transform t)
    {
        target = t;

        if (sprite != null)
            sprite.enabled = (t != null);
    }
}
