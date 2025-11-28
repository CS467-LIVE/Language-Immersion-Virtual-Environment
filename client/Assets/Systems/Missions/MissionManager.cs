using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Systems.Events;

namespace Systems.Missions
{
    public class MissionManager : MonoBehaviour
    {
        [Header("Catalog (drag mission assets here)")]
        public List<MissionDefinition> catalog = new();

        [Header("Runtime (debug)")]
        [SerializeField] private List<MissionRuntime> active = new();
        [SerializeField] private int starsEarnedTotal = 0;

        void OnEnable()
        {
            GameEvents.OnEvent += HandleEvent;
            // For now: activate everything so you can test quickly
            ActivateAll();
        }

        void OnDisable() => GameEvents.OnEvent -= HandleEvent;

        void ActivateAll()
        {
            active.Clear();
            foreach (var def in catalog)
            {
                var mr = new MissionRuntime { missionId = def.missionId };
                foreach (var o in def.objectives)
                    mr.objectives.Add(new ObjectiveState { objectiveId = o.objectiveId, required = Mathf.Max(1, o.targetCount) });

                active.Add(mr);
                Debug.Log($"[Missions] Activated: {def.displayName} ({def.objectives.Count} steps)");
            }
        }

        void HandleEvent(GameEvent e)
        {
            foreach (var mr in active.Where(m => !m.completed))
            {
                var def = catalog.FirstOrDefault(d => d.missionId == mr.missionId);
                if (def == null) continue;

                // Only the current objective is active (sequential flow)
                if (mr.currentIndex < 0 || mr.currentIndex >= def.objectives.Count) continue;

                var oDef = def.objectives[mr.currentIndex];
                var oState = mr.objectives[mr.currentIndex];

                if (Matches(e, oDef))
                {
                    oState.progress += Mathf.Max(1, e.amount);
                    Debug.Log($"[Missions] {def.displayName} — {oDef.objectiveId}: {oState.progress}/{oState.required}");

                    if (oState.progress >= oState.required)
                    {
                        oState.done = true;
                        mr.currentIndex++;

                        if (mr.currentIndex >= def.objectives.Count)
                        {
                            mr.completed = true;
                            starsEarnedTotal += Mathf.Max(0, def.starReward);
                            Debug.Log($"[Missions] COMPLETED: {def.displayName} (+{def.starReward}⭐)  Total Stars: {starsEarnedTotal}");
                        }
                        else
                        {
                            var next = def.objectives[mr.currentIndex];
                            Debug.Log($"[Missions] Next step: {next.objectiveId} — {next.uiHint}");
                        }
                    }
                }
            }
        }

        bool Matches(GameEvent e, ObjectiveDef o)
        {
            switch (o.type)
            {
                case ObjectiveType.AiValidated: return e.type == "AiValidated" && e.subjectId == o.targetId;
                case ObjectiveType.TalkToNPC: return e.type == "TalkedTo" && e.subjectId == o.targetId;
                case ObjectiveType.EnterZone: return e.type == "EnteredZone" && e.subjectId == o.targetId;
                case ObjectiveType.CustomEvent: return e.type == "Custom" && e.subjectId == o.targetId; // use targetId as event key
                default: return false;
            }
        }
    }
}
