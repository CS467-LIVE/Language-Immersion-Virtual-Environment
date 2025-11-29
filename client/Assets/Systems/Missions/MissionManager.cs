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

        [Header("Runtime")]
        [SerializeField] private List<MissionRuntime> active = new();
        [SerializeField] private int starsEarnedTotal = 0;

        void OnEnable()
        {
            GameEvents.OnEvent += HandleEvent;
            ActivateFirst();
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

        void ActivateFirst()
        {
            if (catalog == null || catalog.Count == 0) return;
            ActivateMissionByDef(catalog[0]);
        }

        // Activate a specific mission by definition (clears any previously active)
        public void ActivateMissionByDef(MissionDefinition def)
        {
            if (def == null) return;
            active.Clear();
            var mr = CreateRuntimeFromDef(def);
            active.Add(mr);
            Debug.Log($"[Missions] Activated (single): {def.displayName} ({def.objectives.Count} steps)");
        }

        // Activate the next mission in the catalog after the provided definition (or after the currently active)
        void ActivateNextAfter(MissionDefinition finishedDef)
        {
            if (catalog == null || catalog.Count == 0) return;
            var idx = catalog.IndexOf(finishedDef);
            var nextIdx = idx + 1;
            if (nextIdx >= 0 && nextIdx < catalog.Count)
            {
                ActivateMissionByDef(catalog[nextIdx]);
            }
            else
            {
                // No more missions: clear active
                active.Clear();
                Debug.Log("[Missions] All catalog missions completed or no next mission.");
            }
        }

        MissionRuntime CreateRuntimeFromDef(MissionDefinition def)
        {
            var mr = new MissionRuntime { missionId = def.missionId };
            foreach (var o in def.objectives)
                mr.objectives.Add(new ObjectiveState { objectiveId = o.objectiveId, required = Mathf.Max(1, o.targetCount) });
            return mr;
        }

        void HandleEvent(GameEvent e)
        {
            // snapshot so we don't modify collection while iterating
            var currentMissions = active.Where(m => !m.completed).ToList();

            // Only process the currently active mission(s) (we keep at most one in normal flow)
            foreach (var mr in currentMissions)
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

                            // Automatically activate the next mission in the catalog (if any)
                            ActivateNextAfter(def);
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
                case ObjectiveType.AiValidated: return e.type == "AiValidated" && MatchesTargetId(e.subjectId, o.targetId);
                case ObjectiveType.Interacted: return e.type == "Interacted" && MatchesTargetId(e.subjectId, o.targetId);
                case ObjectiveType.EnterZone: return e.type == "EnteredZone" && MatchesTargetId(e.subjectId, o.targetId);
                case ObjectiveType.CustomEvent: return e.type == "Custom" && MatchesTargetId(e.subjectId, o.targetId);
                default: return false;
            }
        }

        // Matches subject ID against target ID
        // If targetId starts with "ANY_", matches any subjectId that starts with the base type
        // Examples:
        //   "ANY_PHONE_BOOTH" matches "PHONE_BOOTH_1", "PHONE_BOOTH_2", etc.
        //   "ANY_MAILBOX" matches "MAILBOX_1", "MAILBOX_2", etc.
        //   "HOBO" only matches exactly "HOBO"
        bool MatchesTargetId(string subjectId, string targetId)
        {
            if (string.IsNullOrEmpty(subjectId) || string.IsNullOrEmpty(targetId))
                return false;

            // If targetId starts with "ANY_", use matching
            if (targetId.StartsWith("ANY_"))
            {
                // Extract the base type by removing "ANY_" prefix
                string baseType = targetId.Substring(4); // Remove "ANY_"

                // Check if subjectId starts with the base type
                return subjectId.StartsWith(baseType);
            }

            // Otherwise, use exact matching
            return subjectId == targetId;
        }
    }
}
