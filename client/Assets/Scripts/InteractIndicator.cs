using UnityEngine;
using System.Collections;

public class InteractIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas interactIndicator;
    [SerializeField] private Transform player;
    [SerializeField] private ChatboxUI chatbox;
    [SerializeField] private Transform conversationCamPoint;

    [Header("Settings")]
    [SerializeField] private float showDistance = 10f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float camMoveSpeed = 4f;

    private Transform cam;
    private FirstPersonCamera cameraController;
    private PlayerInteraction playerInteraction;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private bool originalPosStored = false;

    private bool inConversation = false;

    void Start()
    {
        cam = Camera.main.transform;
        cameraController = cam.GetComponent<FirstPersonCamera>();
        playerInteraction = player.GetComponent<PlayerInteraction>();

        if (chatbox)
            chatbox.OnChatClosed += EndConversation;

        // Store the camera's original position at start
        if (cam != null && cam.parent == player)
        {
            originalLocalPos = cam.localPosition;
            originalLocalRot = cam.localRotation;
            originalPosStored = true;
        }
    }

    void Update()
    {
        if (!interactIndicator || !player) return;

        // Stop all player interactions during chat
        if (inConversation)
            return;

        // Hide indicator if chat is open
        if (chatbox && chatbox.IsOpen)
        {
            interactIndicator.enabled = false;
            return;
        }

        // Distance check
        float dist = Vector3.Distance(player.position, transform.position);
        bool inRange = dist <= showDistance;
        interactIndicator.enabled = inRange;

        if (!inRange) return;

        // Make indicator face player
        Vector3 targetPos = player.position;
        targetPos.y = interactIndicator.transform.position.y;
        interactIndicator.transform.LookAt(targetPos);
        interactIndicator.transform.Rotate(0f, 180f, 0f);

        // Note: Interaction is now handled by PlayerInteraction + NPCInteractable
    }

    public void BeginConversationCameraMove()
    {
        inConversation = true;

        interactIndicator.enabled = false;   // <<< MINIMAL FIX (THE ONLY CHANGE)

        if (cameraController)
            cameraController.enabled = false;

        if (playerInteraction)
            playerInteraction.enabled = false;

        // Store camera's local info only if not already stored at Start
        if (!originalPosStored && cam.parent == player)
        {
            originalLocalPos = cam.localPosition;
            originalLocalRot = cam.localRotation;
            originalPosStored = true;
        }

        cam.SetParent(null);

        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(conversationCamPoint));
    }

    private IEnumerator MoveCameraTo(Transform target)
    {
        while (Vector3.Distance(cam.position, target.position) > 0.01f)
        {
            cam.position = Vector3.Lerp(cam.position, target.position,
                Time.deltaTime * camMoveSpeed);

            cam.rotation = Quaternion.Lerp(cam.rotation, target.rotation,
                Time.deltaTime * camMoveSpeed);

            yield return null;
        }

        // Lock camera to exact target position and rotation
        cam.position = target.position;
        cam.rotation = target.rotation;

        // Keep camera locked at conversation point while chatting
        while (inConversation)
        {
            cam.position = target.position;
            cam.rotation = target.rotation;
            yield return null;
        }
    }

    private void EndConversation()
    {
        inConversation = false;

        StopAllCoroutines();
        StartCoroutine(ReturnCamera());
    }

    private IEnumerator ReturnCamera()
    {
        // Immediately re-parent camera to player and snap to original position
        cam.SetParent(player);
        cam.localPosition = originalLocalPos;
        cam.localRotation = originalLocalRot;

        // Re-enable controls
        if (cameraController)
            cameraController.enabled = true;

        if (playerInteraction)
            playerInteraction.enabled = true;

        yield return null;
    }
}
