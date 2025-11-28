using UnityEngine;
using System.Collections.Generic;

namespace Systems.Missions
{
    public enum ObjectiveType { AiValidated, Interacted, EnterZone, CustomEvent }

    [CreateAssetMenu(fileName = "Mission", menuName = "Game/Mission")]
    public class MissionDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string missionId;
        public string displayName;

        [Header("Objectives (processed in order)")]
        public List<ObjectiveDef> objectives = new();

        [Header("Reward")]
        public int starReward = 1;
    }

    [System.Serializable]
    public class ObjectiveDef
    {
        public string objectiveId;          // unique within the mission
        public ObjectiveType type;
        public string targetId;             // e.g., NPC id or Zone id
        public int targetCount = 1;         // usually 1
        public string customEventName;      // for CustomEvent (optional UI hint)
        [TextArea] public string uiHint;
    }
}
