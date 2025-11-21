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

    private bool inConversation = false;

    void Start()
    {
        cam = Camera.main.transform;
        cameraController = cam.GetComponent<FirstPersonCamera>();
        playerInteraction = player.GetComponent<PlayerInteraction>();

        if (chatbox)
            chatbox.OnChatClosed += EndConversation;
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

        // Interact
        if (Input.GetKeyDown(interactKey) && chatbox)
        {
            chatbox.OpenChat();
            interactIndicator.enabled = false;
            BeginConversationCameraMove();
        }
    }

    private void BeginConversationCameraMove()
    {
        inConversation = true;

        if (cameraController)
            cameraController.enabled = false;

        if (playerInteraction)
            playerInteraction.enabled = false;

        // Store camera's local info
        originalLocalPos = cam.localPosition;
        originalLocalRot = cam.localRotation;

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

        cam.position = target.position;
        cam.rotation = target.rotation;
    }

    private void EndConversation()
    {
        inConversation = false;

        StopAllCoroutines();
        StartCoroutine(ReturnCamera());
    }

    private IEnumerator ReturnCamera()
    {
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        Vector3 targetWorldPos = player.TransformPoint(originalLocalPos);
        Quaternion targetWorldRot = player.rotation * originalLocalRot;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * camMoveSpeed;
            cam.position = Vector3.Lerp(startPos, targetWorldPos, t);
            cam.rotation = Quaternion.Lerp(startRot, targetWorldRot, t);
            yield return null;
        }

        // Restore camera as child
        cam.SetParent(player);
        cam.localPosition = originalLocalPos;
        cam.localRotation = originalLocalRot;

        if (cameraController)
            cameraController.enabled = true;

        if (playerInteraction)
            playerInteraction.enabled = true;
    }
}
