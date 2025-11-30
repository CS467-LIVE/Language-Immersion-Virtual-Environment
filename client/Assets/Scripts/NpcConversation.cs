using System;
using System.Collections;
using Systems.Events;
using UnityEngine;

public class NpcConversation : MonoBehaviour
{
    [Header("Config")]
    public string npcId = "walletNPC";

    [Header("Runtime state")]
    public int dialogueIndex = 0;
    public string prevRespID = null;

    [Header("Deps")]
    public NpcApiClient apiClient;

    // EVENTS - other systems subscribe to these
    public event Action<NpcConversation, string> OnNpcLine;
    public event Action<NpcConversation, string> OnSystemMessage;
    public event Action<NpcConversation, bool, string> OnEvaluationResult;
    public event Action<NpcConversation, int> OnStepAdvanced;

    // called when player first interacts with NPC
    public void StartConversation()
    {
        if (apiClient == null)
        {
            Debug.LogError($"[NpcConversation] {npcId}: apiClient is NULL! Assign NpcApiClient in Inspector.");
            return;
        }

        // first NPC line, no user input yet
        var req = new DialogueRequest
        {
            npcID = npcId,
            dialogueIndex = dialogueIndex,
            userInput = "",
            prevRespID = null,
            language = GameSettings.CurrentLanguageCode
        };

        StartCoroutine(apiClient.CallDialogue(
            req,
            HandleDialogueSuccess_FirstLine,
            HandleError
        ));
    }

    // called by UI when player submits a line
    public void HandlePlayerInput(string playerText)
    {
        // evaluate first
        var evalReq = new EvaluateRequest
        {
            npcID = npcId,
            dialogueIndex = dialogueIndex,
            userInput = playerText,
            prevRespID = prevRespID,
            language = GameSettings.CurrentLanguageCode
        };

        StartCoroutine(apiClient.CallEvaluation(
            evalReq,
            resp => HandleEvaluationResult(resp, playerText),
            HandleError
        ));
    }

    // ------------- internal handlers -------------

    private void HandleDialogueSuccess_FirstLine(DialogueResponse resp)
    {
        prevRespID = resp.responseID;

        OnNpcLine?.Invoke(this, resp.outputText);
        OnStepAdvanced?.Invoke(this, dialogueIndex);
    }

    // called when we already evaluated and know player was correct
    private void HandleDialogueSuccess_NextStep(DialogueResponse resp)
    {
        prevRespID = resp.responseID;

        OnNpcLine?.Invoke(this, resp.outputText);
        OnStepAdvanced?.Invoke(this, dialogueIndex);
    }

    private void HandleEvaluationResult(EvaluateResponse evalResp, string playerText)
    {
        bool passed = evalResp.passed == "yes";

        OnEvaluationResult?.Invoke(this, passed, evalResp.reason);

        if (!passed)
        {
            OnSystemMessage?.Invoke(this, evalResp.reason);
            return;
        }

        // correct: advance dialogue index first
        dialogueIndex++;

        // notify missions: map dialogueIndex to a key
        string stepKey = $"{npcId}_step_{dialogueIndex}";
        RaiseDialogueStepEvent(stepKey);

        var req = new DialogueRequest
        {
            npcID = npcId,
            dialogueIndex = dialogueIndex,
            userInput = playerText,
            prevRespID = prevRespID,
            language = GameSettings.CurrentLanguageCode
        };

        StartCoroutine(apiClient.CallDialogue(
            req,
            HandleDialogueSuccess_NextStep,
            HandleError
        ));
    }


    private void HandleError(string err)
    {
        Debug.LogError(
        $"NpcConversation error\n" +
        $"  NPC ID: {npcId}\n" +
        $"  Error: {err}\n" +
        $"  Stack: {Environment.StackTrace}"
    );

        OnSystemMessage?.Invoke(this, "Error talking to NPC. Please try again.");
    }

    private void RaiseDialogueStepEvent(string stepKey)
    {
        GameEvents.Raise(new GameEvent
        {
            type = "AiValidated",
            subjectId = stepKey,
            amount = 1
        });
    }
}
