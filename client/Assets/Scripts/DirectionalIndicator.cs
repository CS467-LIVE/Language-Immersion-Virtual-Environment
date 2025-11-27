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

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[DirectionalIndicator] No Camera with 'MainCamera' tag found.");
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

        // Debug when target changes
        if (target != lastTarget)
        {
            if (target)
                Debug.Log($"[DirectionalIndicator] New target set: {target.name}");
            else
                Debug.Log("[DirectionalIndicator] Target cleared.");

            lastTarget = target;
        }

        // No target → hide
        if (target == null)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        // Hide when close to the target
        float dist = Vector3.Distance(cam.transform.position, target.position);
        if (dist <= hideDistance)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        // Ensure visible when valid target & not too close
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // --- Keep arrow locked in front of camera ---
        transform.localPosition = new Vector3(0f, -verticalOffset, distanceInFront);

        // --- Compute horizontal direction to target ---
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 toTarget = target.position - cam.transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        toTarget.Normalize();

        // Signed angle between camera forward and target direction
        float signedAngle = Vector3.SignedAngle(camForward, toTarget, Vector3.up);

        // Arrow sprite points UP, so rotate around local Z
        transform.localRotation = Quaternion.Euler(0f, 0f, -signedAngle);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
