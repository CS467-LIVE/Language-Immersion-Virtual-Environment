using System;

namespace Systems.Missions
{
    [Serializable]
    public class MissionRuntime
    {
        public string missionId;
        public int progress;      // how many times we matched the objective
        public int required = 1;  // copied from definition.targetCount
        public bool completed;
    }
}
