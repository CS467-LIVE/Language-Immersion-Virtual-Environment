using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatboxUI : MonoBehaviour
{
    public event Action OnChatClosed;

    [Header("References")]
    [SerializeField] private Button closeButton;
    [SerializeField] public TMP_Text npcDialogueHistory;
    [SerializeField] public TMP_InputField npcInputField;

    // Renamed as requested
    [SerializeField] private ScrollRect scrollView;

    public bool IsOpen => gameObject.activeSelf;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseChat);
        else
            Debug.LogWarning("ChatboxUI: Close Button not assigned!");
    }

    public void OpenChat()
    {
        gameObject.SetActive(true);

        // Autofocus input
        if (npcInputField != null)
            npcInputField.ActivateInputField();
        Debug.Log("ChatboxUI: Chat opened.");
    }

    public void CloseChat()
    {
        Debug.Log("ChatboxUI: Closing chat...");
        gameObject.SetActive(false);
        Debug.Log("ChatboxUI: Chat closed, invoking event...");
        OnChatClosed?.Invoke();
        Debug.Log("ChatboxUI: Event invoked.");
        Debug.Log("ChatboxUI: Chat closed.");
    }

    // -------------------------------------------------------------
    //  ADD LINE UTILITY (minimal addition)
    // -------------------------------------------------------------
    public void AddLineToDialogue(string line)
    {
        if (npcDialogueHistory == null)
            return;

        npcDialogueHistory.text += line + "<br>";

        // NEW: auto-scroll
        ScrollToBottom();
    }

    // -------------------------------------------------------------
    //  AUTO SCROLL (minimal)
    // -------------------------------------------------------------
    private void ScrollToBottom()
    {
        if (scrollView == null) return;

        // Force layout updates FIRST
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            (RectTransform)npcDialogueHistory.transform
        );

        Canvas.ForceUpdateCanvases();

        scrollView.verticalNormalizedPosition = 0f;

        Canvas.ForceUpdateCanvases();
    }


    // -------------------------------------------------------------
    //  HELPERS FOR NpcDialogueUI (minimal)
    // -------------------------------------------------------------
    public void AddHint(string message)
    {
        AddLineToDialogue($"<color=#FF4444>[Hint] {message}</color>");
    }

    public void AddSystem(string message)
    {
        AddLineToDialogue($"<color=#FF4444>[System] {message}</color>");
    }

    public void AddNpc(string npcName, string msg)
    {
        AddLineToDialogue($"{npcName}: {msg}");
    }

    public void AddPlayer(string msg)
    {
        AddLineToDialogue($"You: {msg}");
    }
}
