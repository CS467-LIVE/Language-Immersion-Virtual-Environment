using UnityEngine;

public class DirectionalIndicator : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Camera Positioning")]
    [SerializeField] private float distanceInFront = 1f;
    [SerializeField] private float verticalOffset = 0.5f;

    [Header("Hiding")]
    [SerializeField] private float hideDistance = 2f;

    private Camera cam;
    private Transform lastTarget;
    private bool initialized = false;
    private SpriteRenderer sprite;   // controls visibility instead of SetActive

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[DirectionalIndicator] No Camera with 'MainCamera' tag found.");
            enabled = false;
            return;
        }

        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite == null)
        {
            Debug.LogError("[DirectionalIndicator] No SpriteRenderer found on indicator.");
            enabled = false;
            return;
        }

        // Attach to camera so it's always visible in view
        transform.SetParent(cam.transform, worldPositionStays: false);

        // Initial position relative to camera
        transform.localPosition = new Vector3(0f, -verticalOffset, distanceInFront);
        transform.localRotation = Quaternion.identity;

        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        // Debug only when target changes
        if (target != lastTarget)
        {
            if (target)
                Debug.Log($"[DirectionalIndicator] New target set: {target.name}");
            else
                Debug.Log("[DirectionalIndicator] Target cleared.");

            lastTarget = target;
        }

        // No target -> just hide sprite but keep script alive
        if (target == null)
        {
            sprite.enabled = false;
            return;
        }

        // ---- Distance-based hiding (only affects sprite, not GameObject) ----
        float dist = Vector3.Distance(cam.transform.position, target.position);
        if (dist <= hideDistance)
        {
            sprite.enabled = false;
        }
        else
        {
            sprite.enabled = true;
        }

        // ---- Positioning ----
        transform.localPosition = new Vector3(0f, -verticalOffset, distanceInFront);

        // ---- Rotation toward target (horizontal only) ----
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 toTarget = target.position - cam.transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        toTarget.Normalize();

        float signedAngle = Vector3.SignedAngle(camForward, toTarget, Vector3.up);

        // Arrow sprite points UP, so rotate around local Z
        transform.localRotation = Quaternion.Euler(0f, 0f, -signedAngle);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // If a new target is set and we're not right on top of it, force show sprite
        if (target != null)
        {
            float dist = Vector3.Distance(cam.transform.position, target.position);
            if (dist > hideDistance && sprite != null)
                sprite.enabled = true;
        }
        else if (sprite != null)
        {
            sprite.enabled = false;
        }

        lastTarget = newTarget;
    }
}
