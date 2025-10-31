using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "NPC";
    public string npcRole = "Citizen";
    
    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player left {npcName}");
        }
    }
    
    public void Interact()
    {
        Debug.Log($"Interacting with {npcName} ({npcRole})");
        // Need to do: Connect to dialogue system
        // Need to do: Connect to mission system
    }
}