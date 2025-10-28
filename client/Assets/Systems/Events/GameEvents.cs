using UnityEngine;
using System;

namespace Systems.Events
{
    public struct GameEvent
    {
        public string type;
        public string subjectId;
        public int amount;
    }

    public class GameEvents
    {
        public static event Action<GameEvent> OnEvent;
        public static void Raise(GameEvent e)
        {
            OnEvent?.Invoke(e);
#if UNITY_EDITOR
            Debug.Log($"[GameEvents] Raised: {e.type} -> {e.subjectId} x{e.amount}");
#endif
        }
    }
}

