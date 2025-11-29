using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Systems.Events;
using TMPro;
using UnityEngine.SceneManagement;

namespace Systems.Missions
{
    public class MissionManager : MonoBehaviour
    {
        [Header("Catalog (drag mission assets here)")]
        public List<MissionDefinition> catalog = new();

        [Header("Runtime")]
        [SerializeField] private List<MissionRuntime> active = new();
        [SerializeField] private int starsEarnedTotal = 0;

        [Header("UI")]
        [SerializeField] private TMP_Text missionTitleText;
        [SerializeField] private TMP_Text missionHintText;
        [SerializeField] private TMP_Text missionBarText;

        void UpdateMissionUI(string title, string hint)
        {
            if (missionTitleText) missionTitleText.text = title;
            if (missionHintText) missionHintText.text = hint;
        }

        void UpdateMissionBar()
        {
            if (active.Count == 0 || missionBarText == null) return;

            var def = active[0].definitionRef;
            int current = Mathf.Clamp(catalog.IndexOf(def) + 1, 1, catalog.Count);
            int total = catalog.Count;

            missionBarText.text = $"Mission {current} out of {total}";
        }

        void Start()
        {
            if (active.Count == 0)
                ActivateFirst();
        }

        void OnEnable()
        {
            GameEvents.OnEvent += HandleEvent;
            ActivateFirst();
        }

        void OnDisable() => GameEvents.OnEvent -= HandleEvent;

        void ActivateFirst()
        {
            if (catalog == null || catalog.Count == 0) return;
            ActivateMissionByDef(catalog[0]);
        }

        public void ActivateMissionByDef(MissionDefinition def)
        {
            active.Clear();

            var mr = CreateRuntimeFromDef(def);
            mr.definitionRef = def;
            active.Add(mr);

            if (def.objectives.Count > 0)
                UpdateMissionUI(def.displayName, def.objectives[0].uiHint);
            else
                UpdateMissionUI(def.displayName, "(No objectives)");

            UpdateMissionBar();
        }

        void ActivateNextAfter(MissionDefinition finishedDef)
        {
            int idx = catalog.IndexOf(finishedDef);
            int nextIdx = idx + 1;

            if (nextIdx >= 0 && nextIdx < catalog.Count)
            {
                ActivateMissionByDef(catalog[nextIdx]);
            }
            else
            {
                SceneManager.LoadScene("EndScene");
            }
        }

        MissionRuntime CreateRuntimeFromDef(MissionDefinition def)
        {
            var mr = new MissionRuntime
            {
                missionId = def.missionId,
                definitionRef = def
            };

            foreach (var o in def.objectives)
                mr.objectives.Add(new ObjectiveState
                {
                    objectiveId = o.objectiveId,
                    required = Mathf.Max(1, o.targetCount)
                });

            return mr;
        }

        void HandleEvent(GameEvent e)
        {
            var currentMissions = active.Where(m => !m.completed).ToList();

            foreach (var mr in currentMissions)
            {
                var def = mr.definitionRef;
                if (def == null) continue;

                if (mr.currentIndex < 0 || mr.currentIndex >= def.objectives.Count) continue;

                var oDef = def.objectives[mr.currentIndex];
                var oState = mr.objectives[mr.currentIndex];

                if (Matches(e, oDef))
                {
                    oState.progress += Mathf.Max(1, e.amount);

                    if (oState.progress >= oState.required)
                    {
                        oState.done = true;
                        mr.currentIndex++;

                        if (mr.currentIndex >= def.objectives.Count)
                        {
                            mr.completed = true;
                            starsEarnedTotal += Mathf.Max(0, def.starReward);
                            ActivateNextAfter(def);
                        }
                        else
                        {
                            var next = def.objectives[mr.currentIndex];
                            UpdateMissionUI(def.displayName, next.uiHint);
                        }

                        UpdateMissionBar();
                    }
                }
            }
        }

        bool Matches(GameEvent e, ObjectiveDef o)
        {
            switch (o.type)
            {
                case ObjectiveType.AiValidated: return e.type == "AiValidated" && e.subjectId == o.targetId;
                case ObjectiveType.Interacted: return e.type == "Interacted" && e.subjectId == o.targetId;
                case ObjectiveType.EnterZone: return e.type == "EnteredZone" && e.subjectId == o.targetId;
                case ObjectiveType.CustomEvent: return e.type == "Custom" && e.subjectId == o.targetId;
                default: return false;
            }
        }
    }
}