using System.Collections;
using UnityEngine;

public class NpcApiTest : MonoBehaviour
{
    public NpcApiClient apiClient;

    private void Start()
    {
        var req = new DialogueRequest
        {
            npcID = "walletNPC",
            dialogueIndex = 0,
            userInput = "",
            prevRespID = null
        };

        StartCoroutine(apiClient.CallDialogue(
            req,
            OnSuccess,
            OnError
        ));
    }

    private void OnSuccess(DialogueResponse resp)
    {
        Debug.Log("NPC outputText: " + resp.outputText);
        Debug.Log("NPC responseID: " + resp.responseID);
    }

    private void OnError(string error)
    {
        Debug.LogError("Error from backend: " + error);
    }
}
