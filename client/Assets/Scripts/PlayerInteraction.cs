using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    private NPCInteractable currentNPC = null;
    private MailboxInteractable currentMailbox = null;
    private PhoneBoothInteractable currentPhoneBooth = null;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // Check for NPC interaction
            if (currentNPC != null)
            {
                currentNPC.Interact();
            }
            // Check for mailbox interaction
            else if (currentMailbox != null)
            {
                currentMailbox.Interact();
            }
            // Check for phone booth interaction
            else if (currentPhoneBooth != null)
            {
                currentPhoneBooth.Interact();
            }
            else
            {
                Debug.Log("E key pressed but nothing nearby to interact with.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for NPC
        NPCInteractable npc = other.GetComponent<NPCInteractable>();
        if (npc != null)
        {
            currentNPC = npc;
            Debug.Log($"Can interact with {npc.npcName}.");
            return;
        }

        // Check for mailbox
        MailboxInteractable mailbox = other.GetComponent<MailboxInteractable>();
        if (mailbox != null)
        {
            currentMailbox = mailbox;
            Debug.Log("Can interact with mailbox.");
            return;
        }

        // Check for phone booth
        PhoneBoothInteractable phoneBooth = other.GetComponent<PhoneBoothInteractable>();
        if (phoneBooth != null)
        {
            currentPhoneBooth = phoneBooth;
            Debug.Log("Can interact with phone booth.");
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check for nPC exit
        NPCInteractable npc = other.GetComponent<NPCInteractable>();
        if (npc != null && npc == currentNPC)
        {
            currentNPC = null;
            return;
        }

        // Check for mailbox exit
        MailboxInteractable mailbox = other.GetComponent<MailboxInteractable>();
        if (mailbox != null && mailbox == currentMailbox)
        {
            currentMailbox = null;
            return;
        }

        // Check for phone booth exit
        PhoneBoothInteractable phoneBooth = other.GetComponent<PhoneBoothInteractable>();
        if (phoneBooth != null && phoneBooth == currentPhoneBooth)
        {
            currentPhoneBooth = null;
            return;
        }
    }
}
