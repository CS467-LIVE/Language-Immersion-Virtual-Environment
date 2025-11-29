using UnityEngine;
using Systems.Events;

public class ZoneTriggerEvent : MonoBehaviour
{
    [Tooltip("The zone ID this trigger will emit, e.g. ZONE_POLICE_STATION")]
    public string zoneId = "ZONE_POLICE_STATION";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.Raise(new GameEvent
            {
                type = "EnteredZone",
                subjectId = zoneId,
                amount = 1
            });

            Debug.Log($"[ZoneTrigger] Player entered {zoneId}");
        }
    }
}
