using UnityEngine;
using Systems.Events;
using UnityEngine.InputSystem; // NEW: Input System namespace

public class HotkeyEventTester : MonoBehaviour
{
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return; // no keyboard attached

        if (kb.cKey.wasPressedThisFrame)
            GameEvents.Raise(new GameEvent { type = "TalkedTo", subjectId = "NPC_CITIZEN_DISTRESSED", amount = 1 });

        if (kb.dKey.wasPressedThisFrame)
            GameEvents.Raise(new GameEvent { type = "Custom", subjectId = "LOST_ITEM_DETAILS", amount = 1 });

        if (kb.pKey.wasPressedThisFrame)
            GameEvents.Raise(new GameEvent { type = "EnteredZone", subjectId = "ZONE_POLICE_STATION", amount = 1 });

        if (kb.oKey.wasPressedThisFrame)
            GameEvents.Raise(new GameEvent { type = "TalkedTo", subjectId = "NPC_POLICE_OFFICER", amount = 1 });

    }
}
