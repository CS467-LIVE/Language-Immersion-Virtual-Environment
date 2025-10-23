# 🎮 Unity Project Style Guide

This document defines naming and organization conventions to keep our Unity project consistent and avoid merge or reference issues.

---

## 🧱 1. General Naming Rules

| Type | Convention | Example |
|------|-------------|----------|
| **Scripts, Classes, Prefabs, Assets** | **PascalCase** (EveryWordCapitalized) | `PlayerController.cs`, `EnemySpawner.prefab` |
| **Variables & Fields** | **camelCase** (firstWordLowercase) | `playerHealth`, `enemyCount` |
| **Constants & Enums** | **ALL_CAPS_WITH_UNDERSCORES** | `MAX_HEALTH`, `GAME_STATE_IDLE` |
| **Folders (we own)** | **PascalCase** | `Integrations`, `MissionSystem`, `UI` |
| **Avoid** | Spaces, special characters | ❌ `My Script.cs`, ✅ `MyScript.cs` |

---

## 🗂️ 2. Folder Structure

```
Assets/
 ├── _Project/                     # Everything we create and own
 │    ├── Scenes/
 │    │    ├── _Sandboxes/         # Personal or feature test scenes
 │    │    └── Game.unity
 │    ├── Scripts/
 │    │    ├── Core/
 │    │    ├── MissionSystem/
 │    │    └── UI/
 │    ├── Integrations/
 │    │    ├── PolygonCity/        # Our prefabs, variants, or overrides that use vendor assets
 │    │    └── OtherVendor/
 │    ├── ScriptableObjects/
 │    ├── Tools/Editor/
 │    └── Assemblies/              # asmdefs (Runtime, Editor, Tests)
 │
 ├── _Vendors/                     # Store-bought packs, unmodified
 │    ├── PolygonCity/
 │    └── OtherPaidPack/
 │
 └── Plugins/                      # Only for plugins that require this path (rare)
```

---

## 🧩 3. Asset Naming Conventions

💡 We do not create original art, models, or audio.
All store-bought assets stay exactly as imported under `_Vendors/`.
We never rename or reorganize vendor files.

---

## ⚙️ 4. Script Naming

- Each `.cs` file must **match its class name** (Unity requirement).  
  ✅ `PlayerController.cs` contains `class PlayerController`
- Group scripts by **feature**, not by type.  
  ❌ `Scripts/Managers/AllManagers.cs`  
  ✅ `Scripts/MissionSystem/MissionManager.cs`

---

## 🧠 5. Scene Management

- **Name by purpose, not number:**  
  `MainMenu.unity`, `Town.unity`, `HouseA.unity`, `HouseB.unity`

- **Use folders for grouping:**  
  e.g. `Scenes/Town/Town.unity`, `Scenes/Town/HouseA.unity`

- **Keep personal/feature test scenes in `_Tests/`**  
  so teammates can work independently.  
  e.g. `Scenes/_Tests/Test_MissionSystem.unity`
---

## ✅ 6. Quick Reference Summary

| Category | Case | Example |
|-----------|------|----------|
| **Scripts / Classes** | PascalCase | `PlayerController` |
| **Variables** | camelCase | `moveSpeed` |
| **Constants** | ALL_CAPS | `MAX_ENEMIES` |
<<<<<<< HEAD
| **Assets** | Context_Suffix | `Player_Mat`, `EnemyGoblin.prefab` |
| **Scenes** | Purpose | `MainMenu` |
=======
| **Scenes** | PascalCase | `MainMenu` |
>>>>>>> 1d3371f (Refactor Unity project style guide for only vendor asset use)
| **ScriptableObjects** | SO_Purpose | `SO_WeaponStats` |

---
