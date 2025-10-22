# 🎮 Unity Project Style Guide

This document defines naming and organization conventions to keep our Unity project consistent and avoid merge or reference issues.

---

## 🧱 1. General Naming Rules

| Type | Convention | Example |
|------|-------------|----------|
| **Scripts, Classes, Prefabs, Assets** | **PascalCase** (EveryWordCapitalized) | `PlayerController.cs`, `EnemySpawner.prefab` |
| **Variables & Fields** | **camelCase** (firstWordLowercase) | `playerHealth`, `enemyCount` |
| **Constants & Enums** | **ALL_CAPS_WITH_UNDERSCORES** | `MAX_HEALTH`, `GAME_STATE_IDLE` |
| **File names that mirror data** | **snake_case** | `player_stats.json`, `level_data_01.txt` |
| **Avoid** | Spaces, special characters | ❌ `My Script.cs`, ✅ `MyScript.cs` |

---

## 🗂️ 2. Folder Structure

```
Assets/
 ├── Art/
 │    ├── Models/
 │    ├── Materials/
 │    └── Textures/
 ├── Audio/
 ├── Prefabs/
 ├── Scenes/
 ├── Scripts/
 │    ├── Player/
 │    ├── Enemies/
 │    ├── UI/
 │    └── Managers/
 ├── UI/
 │    ├── Sprites/
 │    ├── Fonts/
 │    └── Prefabs/
 ├── ScriptableObjects/
 └── Plugins/
```

💡 *Subsystems (like “Inventory” or “MissionSystem”) should each have their own folder under `Scripts/` and `Prefabs/` if applicable.*

---

## 🧩 3. Asset Naming Conventions

| Asset Type | Convention | Example |
|-------------|-------------|----------|
| **Scenes** | Purpose | `MainMenu` |
| **Prefabs** | Descriptive noun | `Player.prefab`, `EnemyGoblin.prefab` |
| **Materials** | Suffix `_Mat` | `Player_Mat.mat` |
| **Textures** | Suffix `_Tex`, optional resolution | `Ground_Tex_2K.png` |
| **Models** | Suffix `_Model` or `_FBX` | `Tree_Model.fbx` |
| **Animations** | `_Anim` for clips, `_AC` for controllers | `Run_Anim.anim`, `Player_AC.controller` |
| **Audio** | Prefix by category + suffix `_SFX` or `_BGM` | `UI_Click_SFX.wav`, `Battle_BGM.mp3` |
| **ScriptableObjects** | Prefix with `SO_` | `SO_WeaponStats.asset`, `SO_LevelData.asset` |
| **Shaders** | Suffix `_Shader` | `Water_Shader.shader` |
| **Sprites/UI** | Prefix by context | `UI_Button_Normal.png`, `HUD_HealthBar.png` |

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
| **Assets** | Context_Suffix | `Player_Mat`, `EnemyGoblin.prefab` |
| **Scenes** | Purpose | `MainMenu` |
| **ScriptableObjects** | SO_Purpose | `SO_WeaponStats` |

---
