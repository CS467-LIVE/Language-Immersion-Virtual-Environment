[System.Serializable]
public class DialogueRequest
{
    public string npcID;
    public int dialogueIndex;
    public string userInput;
    public string prevRespID;
}

[System.Serializable]
public class DialogueResponse
{
    // Adjust field names after you see his JSON
    public string outputText;
    public string responseID;
}

[System.Serializable]
public class EvaluateRequest
{
    public string npcID;
    public int dialogueIndex;
    public string userInput;
    public string prevRespID;
}

[System.Serializable]
public class EvaluateResponse
{
    public string passed;   // "yes" or "no"
    public string reason;   // explanation text
}
