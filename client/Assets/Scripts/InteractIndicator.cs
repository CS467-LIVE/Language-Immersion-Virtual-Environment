using UnityEngine;
using System.Collections;

public class InteractIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas interactIndicator;
    [SerializeField] private Transform player;              // Camera's parent (Player object)
    [SerializeField] private ChatboxUI chatbox;
    [SerializeField] private Transform conversationCamPoint;

    [Header("Settings")]
    [SerializeField] private float showDistance = 10f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float camMoveSpeed = 4f;

    private Transform cam;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private bool inConversation = false;

    void Start()
    {
        cam = Camera.main.transform;

        if (chatbox)
            chatbox.OnChatClosed += EndConversation;
    }

    void Update()
    {
        if (!interactIndicator || !player) return;

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

        // Face player (stable)
        Vector3 targetPos = player.position;
        targetPos.y = interactIndicator.transform.position.y;
        interactIndicator.transform.LookAt(targetPos);
        interactIndicator.transform.Rotate(0f, 180f, 0f);

        // Open chat
        if (Input.GetKeyDown(interactKey) && chatbox)
        {
            chatbox.OpenChat();
            interactIndicator.enabled = false;
            BeginConversationCameraMove();
        }
    }

    private void BeginConversationCameraMove()
    {
        // Save LOCAL transform before detaching
        originalLocalPos = cam.localPosition;
        originalLocalRot = cam.localRotation;

        // Detach so it can move freely
        cam.SetParent(null);

        inConversation = true;

        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(conversationCamPoint));
    }

    private IEnumerator MoveCameraTo(Transform target)
    {
        while (inConversation)
        {
            cam.position = Vector3.Lerp(cam.position, target.position, Time.deltaTime * camMoveSpeed);
            cam.rotation = Quaternion.Lerp(cam.rotation, target.rotation, Time.deltaTime * camMoveSpeed);
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

        // Reattach and restore local transform
        cam.SetParent(player);
        cam.localPosition = originalLocalPos;
        cam.localRotation = originalLocalRot;
    }
}
