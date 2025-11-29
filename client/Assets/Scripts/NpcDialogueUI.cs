using TMPro;
using UnityEngine;

public class NpcDialogueUI : MonoBehaviour
{
    [Header("Visual UI")]
    [Tooltip("Reference to Jackie's ChatboxUI for visual display")]
    public ChatboxUI chatboxUI;

    [SerializeField]
    private NpcConversation currentNpc;

    void Update()
    {
        // Only trigger when:
        //  - chatbox is open
        //  - the input field has focus
        //  - player hits Enter (Return or keypad Enter)
        if (chatboxUI != null &&
            chatboxUI.IsOpen &&
            chatboxUI.npcInputField != null &&
            chatboxUI.npcInputField.isFocused &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SubmitPlayerMessage();
        }
    }

    public void SetActiveNpc(NpcConversation npc)
    {
        // unsubscribe from old
        if (currentNpc != null)
        {
            currentNpc.OnNpcLine -= HandleNpcLine;
            currentNpc.OnSystemMessage -= HandleSystemMessage;
            currentNpc.OnEvaluationResult -= HandleEval;
        }

        currentNpc = npc;

        if (currentNpc != null)
        {
            currentNpc.OnNpcLine += HandleNpcLine;
            currentNpc.OnSystemMessage += HandleSystemMessage;
            currentNpc.OnEvaluationResult += HandleEval;

            // Clear previous dialogue
            if (chatboxUI != null && chatboxUI.npcDialogueHistory != null)
            {
                chatboxUI.npcDialogueHistory.text = "";
            }

            // Open the visual chatbox
            if (chatboxUI != null)
            {
                chatboxUI.OpenChat();
            }

            currentNpc.StartConversation();
        }
    }

    public void SubmitPlayerMessage()
    {
        Debug.Log("SubmitPlayerMessage called");
        if (currentNpc == null) return;
        string text = chatboxUI.npcInputField.text;
        if (string.IsNullOrWhiteSpace(text)) return;

        AppendLine($"You: {text}");
        currentNpc.HandlePlayerInput(text);
        chatboxUI.npcInputField.text = "";
    }

    private void HandleNpcLine(NpcConversation npc, string line)
    {
        AppendLine($"{npc.npcId}: {line}");
    }

    private void HandleSystemMessage(NpcConversation npc, string msg)
    {
        AppendLine($"[System] {msg}");
    }

    private void HandleEval(NpcConversation npc, bool passed, string reason)
    {
        if (!passed)
        {
            AppendLine($"[Hint] {reason}");
        }
    }

    private void AppendLine(string line)
    {
        if (chatboxUI == null || chatboxUI.npcDialogueHistory == null)
        {
            Debug.LogError("[NpcDialogueUI] chatboxUI or npcDialogueHistory is NULL!");
            return;
        }
        chatboxUI.npcDialogueHistory.text += line + "\n";
    }
}
