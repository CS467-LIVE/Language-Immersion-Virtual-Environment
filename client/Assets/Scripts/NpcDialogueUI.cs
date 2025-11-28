using TMPro;
using UnityEngine;

public class NpcDialogueUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text dialogueHistory;

    [SerializeField]
    private NpcConversation currentNpc;

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

            currentNpc.StartConversation();
        }
    }

    public void OnSendButtonClicked()
    {
        if (currentNpc == null) return;
        string text = inputField.text;
        if (string.IsNullOrWhiteSpace(text)) return;

        AppendLine($"You: {text}");
        currentNpc.HandlePlayerInput(text);
        inputField.text = "";
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
        dialogueHistory.text += line + "\n";
    }
}
