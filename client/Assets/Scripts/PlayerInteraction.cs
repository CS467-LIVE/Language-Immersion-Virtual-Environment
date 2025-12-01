using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    private NPCInteractable currentNPC = null;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (currentNPC != null)
            {
                currentNPC.Interact();
            }
            else
            {
                Debug.Log("E key pressed but no NPC nearby.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check both the object itself and its parent for NPCInteractable
        NPCInteractable npc = other.GetComponent<NPCInteractable>();
        if (npc == null)
        {
            npc = other.GetComponentInParent<NPCInteractable>();
        }

        if (npc != null)
        {
            currentNPC = npc;
            //Debug.Log($"Can interact with {npc.conversation.npcName}.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check both the object itself and its parent for NPCInteractable
        NPCInteractable npc = other.GetComponent<NPCInteractable>();
        if (npc == null)
        {
            npc = other.GetComponentInParent<NPCInteractable>();
        }

        if (npc != null && npc == currentNPC)
        {
            currentNPC = null;
        }
    }
}
