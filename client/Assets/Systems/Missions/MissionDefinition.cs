using UnityEngine;

namespace Systems.Missions
{
    public enum ObjectiveType { BuyItem, TalkToNPC, EnterZone }

    [CreateAssetMenu(fileName = "Mission", menuName = "Game/Mission")]
    public class MissionDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string missionId = "buy_hotdog";
        public string displayName = "Buy a Hot Dog";

        [Header("Objective (single, simple)")]
        public ObjectiveType type = ObjectiveType.BuyItem;
        public string targetId = "HOT_DOG"; // must match your event subjectId
        public int targetCount = 1;         // how many times needed

        [Header("Reward")]
        public int starReward = 1;
    }
}
