using UnityEngine;
using Systems.Events;

public class HotkeyEventTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GameEvents.Raise(new GameEvent { type = "Interacted", subjectId = "NPC_HOTDOG_SELLER", amount = 1 });

        if (Input.GetKeyDown(KeyCode.T))
            GameEvents.Raise(new GameEvent { type = "AiValidated", subjectId = "NPC_HOTDOG_SELLER", amount = 1 });

        if (Input.GetKeyDown(KeyCode.Y))
            GameEvents.Raise(new GameEvent { type = "Interacted", subjectId = "NPC_CITIZEN_DISTRESSED", amount = 1 });

        if (Input.GetKeyDown(KeyCode.U))
            GameEvents.Raise(new GameEvent { type = "AiValidated", subjectId = "NPC_CITIZEN_DISTRESSED", amount = 1 });

        if (Input.GetKeyDown(KeyCode.I))
            GameEvents.Raise(new GameEvent { type = "EnteredZone", subjectId = "ZONE_POLICE_STATION", amount = 1 });

        if (Input.GetKeyDown(KeyCode.O))
            GameEvents.Raise(new GameEvent { type = "AiValidated", subjectId = "NPC_POLICE_OFFICER", amount = 1 });

        if (Input.GetKeyDown(KeyCode.H))
            GameEvents.Raise(new GameEvent { type = "Interacted", subjectId = "HOBO", amount = 1 });

        if (Input.GetKeyDown(KeyCode.J))
            GameEvents.Raise(new GameEvent { type = "AiValidated", subjectId = "HOBO", amount = 1 });

        if (Input.GetKeyDown(KeyCode.K))
            GameEvents.Raise(new GameEvent { type = "Interacted", subjectId = "ANY_PHONE_BOOTH", amount = 1 });

        if (Input.GetKeyDown(KeyCode.L))
            GameEvents.Raise(new GameEvent { type = "AiValidated", subjectId = "ANY_PHONE_BOOTH", amount = 1 });

        if (Input.GetKeyDown(KeyCode.V))
            GameEvents.Raise(new GameEvent { type = "Interacted", subjectId = "OLD_LADY", amount = 1 });

        if (Input.GetKeyDown(KeyCode.B))
            GameEvents.Raise(new GameEvent { type = "AiValidated", subjectId = "OLD_LADY", amount = 1 });

        if (Input.GetKeyDown(KeyCode.N))
            GameEvents.Raise(new GameEvent { type = "Deposit", subjectId = "MAILBOX", amount = 1 });
    }
}
