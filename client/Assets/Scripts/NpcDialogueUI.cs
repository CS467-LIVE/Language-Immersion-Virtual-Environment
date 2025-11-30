using TMPro;
using UnityEngine;

public class NpcDialogueUI : MonoBehaviour
{
    [Header("Visual UI")]
    public ChatboxUI chatboxUI;

    private NpcConversation currentNpc;

    void Update()
    {
        // ENTER submits message
        if (chatboxUI != null &&
            chatboxUI.IsOpen &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SubmitPlayerMessage();
        }
    }

    public void SetActiveNpc(NpcConversation npc)
    {
        // Unsubscribe
        if (currentNpc != null)
        {
            currentNpc.OnNpcLine -= HandleNpcLine;
            currentNpc.OnSystemMessage -= HandleSystemMessage;
            currentNpc.OnEvaluationResult -= HandleEval;
        }

        currentNpc = npc;

        if (currentNpc != null)
        {
            // Clear dialogue
            if (chatboxUI.npcDialogueHistory != null)
                chatboxUI.npcDialogueHistory.text = "";

            chatboxUI.OpenChat();

            // Subscribe new NPC
            currentNpc.OnNpcLine += HandleNpcLine;
            currentNpc.OnSystemMessage += HandleSystemMessage;
            currentNpc.OnEvaluationResult += HandleEval;

            currentNpc.StartConversation();
        }
    }

    public void SubmitPlayerMessage()
    {
        if (currentNpc == null) return;

        string text = chatboxUI.npcInputField.text;
        if (string.IsNullOrWhiteSpace(text)) return;

        chatboxUI.AddPlayer(text);
        currentNpc.HandlePlayerInput(text);

        chatboxUI.npcInputField.text = "";
        chatboxUI.npcInputField.ActivateInputField();
    }

    private void HandleNpcLine(NpcConversation npc, string line)
    {
        chatboxUI.AddNpc(npc.npcName, line);
    }

    private void HandleSystemMessage(NpcConversation npc, string msg)
    {
        chatboxUI.AddSystem(msg);
    }

    private void HandleEval(NpcConversation npc, bool passed, string reason)
    {
        if (!passed)
            chatboxUI.AddHint(reason);
    }
}
