using UnityEngine;

public class ChatboxUI : MonoBehaviour
{
    public System.Action OnChatClosed;

    // Chatbox is considered "open" when the GameObject is active
    public bool IsOpen => gameObject.activeSelf;

    public void OpenChat()
    {
        gameObject.SetActive(true);
    }

    public void CloseChat()
    {
        gameObject.SetActive(false);
        OnChatClosed?.Invoke();
    }
}
