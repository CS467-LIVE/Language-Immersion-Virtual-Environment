using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    
    private NPCInteractable currentNPC = null;
    
    void Update()
    {
        // Handle interaction input
        if (Input.GetKeyDown(interactKey))
        {
            if (currentNPC != null)
            {
                currentNPC.Interact();
            }
            else
            {
                // If no NPC is nearby
                Debug.Log("E key pressed but no NPC nearby");
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        NPCInteractable npc = other.GetComponent<NPCInteractable>();
        if (npc != null)
        {
            currentNPC = npc;
            // In range to press E to interact
            Debug.Log($"Can interact with {npc.npcName}");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        NPCInteractable npc = other.GetComponent<NPCInteractable>();
        if (npc != null && npc == currentNPC)
        {
            // Left interaction range
            currentNPC = null;
        }
    }
}