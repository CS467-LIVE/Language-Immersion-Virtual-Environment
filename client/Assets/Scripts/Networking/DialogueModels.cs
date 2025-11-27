[System.Serializable]
public class DialogueRequest
{
    public string npcName;
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
