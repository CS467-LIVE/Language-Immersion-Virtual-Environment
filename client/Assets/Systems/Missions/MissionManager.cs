using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Systems.Events;   // from Step 1
                        // (GameEvents + GameEvent)
namespace Systems.Missions
{
    public class MissionManager : MonoBehaviour
    {
        [Header("Catalog (drag your Mission assets here)")]
        public List<MissionDefinition> catalog = new();

        [Header("Runtime (debug view)")]
        [SerializeField] private List<MissionRuntime> active = new();
        [SerializeField] private int starsEarnedTotal = 0;

        private int totalStarsInGame;

        void Awake()
        {
            totalStarsInGame = catalog.Sum(m => Mathf.Max(0, m.starReward));
        }

        void OnEnable()
        {
            GameEvents.OnEvent += HandleEvent;
            // For Step 2, auto-activate all missions in catalog so you can test quickly.
            AutoActivateAll();
        }

        void OnDisable()
        {
            GameEvents.OnEvent -= HandleEvent;
        }

        private void AutoActivateAll()
        {
            active.Clear();
            foreach (var def in catalog)
            {
                var mr = new MissionRuntime
                {
                    missionId = def.missionId,
                    progress = 0,
                    required = Mathf.Max(1, def.targetCount),
                    completed = false
                };
                active.Add(mr);
                Debug.Log($"[Missions] Activated: {def.displayName} (need {mr.required} x {def.targetId})");
            }
        }

        private void HandleEvent(GameEvent e)
        {
            // Check all active missions for a match
            foreach (var mr in active.Where(a => !a.completed))
            {
                var def = catalog.FirstOrDefault(d => d.missionId == mr.missionId);
                if (def == null) continue;

                if (Matches(def, e))
                {
                    mr.progress += Mathf.Max(1, e.amount);
                    Debug.Log($"[Missions] Progress {def.displayName}: {mr.progress}/{mr.required}");

                    if (mr.progress >= mr.required)
                    {
                        mr.completed = true;
                        starsEarnedTotal += Mathf.Max(0, def.starReward);
                        Debug.Log($"[Missions] COMPLETED: {def.displayName} (+{def.starReward}⭐)  Total Stars: {starsEarnedTotal}/{totalStarsInGame}");
                        CheckForCredits();
                    }
                }
            }
        }

        private bool Matches(MissionDefinition def, GameEvent e)
        {
            switch (def.type)
            {
                case ObjectiveType.BuyItem: return e.type == "BoughtItem" && e.subjectId == def.targetId;
                case ObjectiveType.TalkToNPC: return e.type == "TalkedTo" && e.subjectId == def.targetId;
                case ObjectiveType.EnterZone: return e.type == "EnteredZone" && e.subjectId == def.targetId;
                default: return false;
            }
        }

        private void CheckForCredits()
        {
            if (starsEarnedTotal >= totalStarsInGame && totalStarsInGame > 0)
            {
                Debug.Log("[Missions] All stars collected — roll credits!");
                // SceneManager.LoadScene("Credits"); // later
            }
        }
    }
}
