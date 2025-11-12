using UnityEngine;
using Systems.Events;

public class HotkeyEventTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            GameEvents.Raise(new GameEvent { type = "TalkedTo", subjectId = "NPC_CITIZEN_DISTRESSED", amount = 1 });

        if (Input.GetKeyDown(KeyCode.B))
            GameEvents.Raise(new GameEvent { type = "Custom", subjectId = "LOST_ITEM_DETAILS", amount = 1 });

        if (Input.GetKeyDown(KeyCode.N))
            GameEvents.Raise(new GameEvent { type = "EnteredZone", subjectId = "ZONE_POLICE_STATION", amount = 1 });

        if (Input.GetKeyDown(KeyCode.M))
            GameEvents.Raise(new GameEvent { type = "TalkedTo", subjectId = "NPC_POLICE_OFFICER", amount = 1 });
    }
}
