using UnityEngine;

public class NpcConversation : MonoBehaviour
{

    public string npcName = "walletNPC";
    public int dialogueIndex = 0;
    public string prevRespID = null;

    public NpcApiClient apiClient;
    public NpcDialogueUI dialogueUi;

    public void PlayerSaid(string playerText)
    {
        var req = new DialogueRequest
        {
            npcName = npcName,
            dialogueIndex = dialogueIndex,
            userInput = playerText,
            prevRespID = prevRespID
        };

        StartCoroutine(apiClient.CallDialogue(
            req,
            OnDialogueSuccess,
            OnDialogueError
        ));
    }

    private void OnDialogueSuccess(DialogueResponse resp)
    {
        prevRespID = resp.responseID;
        dialogueUi.AppendNpcLine(npcName, resp.outputText);
    }

    private void OnDialogueError(string error)
    {
        dialogueUi.AppendSystemMessage("Error: " + error);
    }
}
