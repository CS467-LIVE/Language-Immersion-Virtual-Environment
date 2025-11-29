using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatboxUI : MonoBehaviour
{
    public event Action OnChatClosed;

    [Header("References")]
    [SerializeField] private Button closeButton;
    [SerializeField] public TMP_Text npcDialogueHistory;
    [SerializeField] public TMP_InputField npcInputField;

    public bool IsOpen => gameObject.activeSelf;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseChat);
        else
            Debug.LogWarning("ChatboxUI: Close Button not assigned!");
    }

    public void OpenChat()
    {
        Debug.Log("OpenChat called");
        gameObject.SetActive(true);
    }

    public void CloseChat()
    {
        Debug.Log("CloseChat called");
        gameObject.SetActive(false);
        OnChatClosed?.Invoke();
    }
}
