using System;
using System.Collections.Generic;

namespace Systems.Missions
{
    [Serializable]
    public class MissionRuntime
    {
        public string missionId;
        public List<ObjectiveState> objectives = new();
        public int currentIndex = 0;
        public bool completed;
    }

    [Serializable]
    public class ObjectiveState
    {
        public string objectiveId;
        public int progress = 0;
        public int required = 1;
        public bool done = false;
    }
}
