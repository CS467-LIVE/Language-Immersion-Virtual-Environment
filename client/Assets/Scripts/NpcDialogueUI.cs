using TMPro;
using UnityEngine;

public class NpcDialogueUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text dialogueHistory;

    public NpcConversation activeNpc;

    public void OnSendButtonClicked()
    {
        if (activeNpc == null)
        {
            AppendSystemMessage("[No NPC selected]");
            return;
        }

        string text = inputField.text;
        if (string.IsNullOrWhiteSpace(text)) return;

        AppendLine("You: " + text);
        activeNpc.PlayerSaid(text);
        inputField.text = "";
    }

    public void AppendNpcLine(string npcName, string line)
    {
        AppendLine(npcName + ": " + line);
    }

    public void AppendSystemMessage(string msg)
    {
        AppendLine(msg);
    }

    private void AppendLine(string line)
    {
        dialogueHistory.text += line + "\n";
    }
}
