using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class SyntyBulkImportAndNormalize
{
    // Heuristics for Synty roots you may see after import
    static readonly string[] KnownSyntyRoots = new[]
    {
        "Synty", "SYNTY",
        "Polygon", "POLYGON",
        "PolygonCity", "PolygonShops", "PolygonTown",
        "PolygonGeneric", "POLYGON_Generic",
        "Simple", // for Simple Series packs
    };

    public static void NormalizeOnly()
    {
        try
        {
            EnsureSyntyRoot();
            NormalizeAllSyntyRoots();
            Debug.Log("✅ NormalizeOnly: done.");
        }
        finally
        {
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
    }

    // ===== Normalization =====
    static void NormalizeAllSyntyRoots()
    {
        EnsureSyntyRoot();

        // 1) Top-level folders that look like Synty packs
        var topLevel = AssetDatabase.GetSubFolders("Assets")
            .Where(p => p != "Assets/Synty" && p != "Assets/Packages" && p != "Assets/Editor")
            .ToArray();

        foreach (var path in topLevel)
        {
            if (!LooksLikeSyntyRoot(path)) continue;

            string packName = GetPackName(path);
            string target = $"Assets/Synty/{packName}";

            if (path.Replace("\\", "/").Equals(target, System.StringComparison.OrdinalIgnoreCase))
                continue;

            MergeOrMoveFolder(path, target);
        }

        // 2) Collapse nested “Assets/Synty/PolygonCity/PolygonCity”
        foreach (var sub in AssetDatabase.GetSubFolders("Assets/Synty"))
        {
            string name = Path.GetFileName(sub);
            string nested = Path.Combine(sub, name).Replace("\\", "/");
            if (AssetDatabase.IsValidFolder(nested))
                MergeOrMoveFolder(nested, sub);
        }

        // 3) Catch loose Synty assets directly in Assets/
        var looseAssets = AssetDatabase.FindAssets("", new[] { "Assets" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !AssetDatabase.IsValidFolder(p) && Path.GetDirectoryName(p) == "Assets")
            .ToArray();

        if (looseAssets.Length > 0)
        {
            string unwrappedDir = "Assets/Synty/_Unwrapped";
            EnsureFolder(unwrappedDir);

            foreach (var asset in looseAssets)
            {
                string fileName = Path.GetFileName(asset);
                if (fileName.StartsWith("Polygon", System.StringComparison.OrdinalIgnoreCase) ||
                    fileName.StartsWith("Synty", System.StringComparison.OrdinalIgnoreCase) ||
                    fileName.StartsWith("Simple", System.StringComparison.OrdinalIgnoreCase))
                {
                    string dest = Path.Combine(unwrappedDir, fileName).Replace("\\", "/");
                    string res = AssetDatabase.MoveAsset(asset, dest);
                    if (string.IsNullOrEmpty(res))
                        Debug.Log($"📦 Moved loose file {fileName} → {unwrappedDir}");
                    else
                        Debug.LogWarning($"⚠️ Couldn’t move {fileName}: {res}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ Normalization complete (folders + loose files).");
    }

    static bool LooksLikeSyntyRoot(string path)
    {
        string name = Path.GetFileName(path).ToLowerInvariant();
        if (KnownSyntyRoots.Any(k => name.Contains(k.ToLowerInvariant()))) return true;

        int hits = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:prefab t:material t:mesh", new[] { path }))
            if (++hits > 20) return true;

        return false;
    }

    static string GetPackName(string path)
    {
        string name = Path.GetFileName(path);
        name = name.Replace("SYNTY_", "").Replace("Synty_", "");
        name = name.Replace("POLYGON_", "Polygon").Replace("POLYGON - ", "Polygon ").Replace("POLYGON ", "Polygon ");
        return name.Trim();
    }

    static void MergeOrMoveFolder(string from, string to)
    {
        EnsureFolder(Path.GetDirectoryName(to).Replace("\\", "/"));

        if (!AssetDatabase.IsValidFolder(to))
        {
            var res = AssetDatabase.MoveAsset(from, to);
            if (string.IsNullOrEmpty(res))
            {
                Debug.Log($"📁 Moved: {from} → {to}");
            }
            else
            {
                Debug.LogWarning($"Move failed; attempting merge. {res}");
                MergeIntoExisting(from, to);
            }
            return;
        }
        MergeIntoExisting(from, to);
    }

    static void MergeIntoExisting(string from, string to)
    {
        var children = AssetDatabase.FindAssets("", new[] { from })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => Path.GetDirectoryName(p).Replace("\\", "/").StartsWith(from + "/"))
            .OrderBy(p => p)
            .ToList();

        foreach (var childPath in children)
        {
            if (AssetDatabase.IsValidFolder(childPath))
            {
                string rel = childPath.Substring(from.Length).TrimStart('/');
                EnsureFolder(CombineFolders(to, rel));
            }
        }

        foreach (var childPath in children)
        {
            if (AssetDatabase.IsValidFolder(childPath)) continue;
            string rel = childPath.Substring(from.Length).TrimStart('/');
            string dest = CombineFolders(to, rel);

            EnsureFolder(Path.GetDirectoryName(dest).Replace("\\", "/"));

            var res = AssetDatabase.MoveAsset(childPath, dest);
            if (!string.IsNullOrEmpty(res))
            {
                string alt = Path.Combine(Path.GetDirectoryName(dest) ?? to,
                    Path.GetFileNameWithoutExtension(dest) + "_dup" + Path.GetExtension(dest)).Replace("\\", "/");
                res = AssetDatabase.MoveAsset(childPath, alt);
                if (!string.IsNullOrEmpty(res))
                    Debug.LogWarning($"⚠️ Couldn’t move {childPath} → {dest}: {res}");
            }
        }

        TryDeleteFolderIfEmpty(from);
        Debug.Log($"🔀 Merged: {from} → {to}");
    }

    static void EnsureSyntyRoot()
    {
        if (!AssetDatabase.IsValidFolder("Assets"))
        {
            Debug.LogError("❌ Unity project missing 'Assets' folder.");
            return;
        }
        if (!AssetDatabase.IsValidFolder("Assets/Synty"))
        {
            AssetDatabase.CreateFolder("Assets", "Synty");
            Debug.Log("📁 Created 'Assets/Synty' root.");
            AssetDatabase.Refresh();
        }
    }

    static void EnsureFolder(string folder)
    {
        folder = folder.Replace("\\", "/");
        if (string.IsNullOrEmpty(folder) || folder == "Assets") return;

        var parent = Path.GetDirectoryName(folder)?.Replace("\\", "/") ?? "Assets";
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
    }

    static string CombineFolders(string a, string b)
        => (a.TrimEnd('/') + "/" + b.TrimStart('/')).Replace("\\", "/");

    static void TryDeleteFolderIfEmpty(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder))
        {
            var ok = AssetDatabase.DeleteAsset(folder);
            // ok true if deleted (empty), false otherwise
        }
    }
}
