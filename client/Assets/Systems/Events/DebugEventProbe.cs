using UnityEngine;
using Systems.Events;

public class DebugEventProbe : MonoBehaviour
{
    void OnEnable() => GameEvents.OnEvent += Handle;
    void OnDisable() => GameEvents.OnEvent -= Handle;

    private void Handle(GameEvent e)
    {
        Debug.Log($"[Probe] Heard event: {e.type} ({e.subjectId}) amount={e.amount}");
    }
}
