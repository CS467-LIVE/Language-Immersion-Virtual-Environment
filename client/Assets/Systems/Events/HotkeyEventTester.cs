using UnityEngine;
using Systems.Events;
using UnityEngine.InputSystem; // NEW: Input System namespace

public class HotkeyEventTester : MonoBehaviour
{
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return; // no keyboard attached

        if (kb.hKey.wasPressedThisFrame)
        {
            GameEvents.Raise(new GameEvent { type = "BoughtItem", subjectId = "HOT_DOG", amount = 1 });
        }
        if (kb.tKey.wasPressedThisFrame)
        {
            GameEvents.Raise(new GameEvent { type = "TalkedTo", subjectId = "NPC_VENDOR_1", amount = 1 });
        }
        if (kb.zKey.wasPressedThisFrame)
        {
            GameEvents.Raise(new GameEvent { type = "EnteredZone", subjectId = "ZONE_MARKET", amount = 1 });
        }
    }
}
