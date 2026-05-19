using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Game.Tools
{
    public static class ArchitectureValidator
    {
        [MenuItem("Tools/Run Architecture Validation")]
        public static void RunValidation()
        {
            var errors = new List<string>();
            var domainFiles = Directory.GetFiles(
                "Assets/Game/Script/Domain", "*.cs", SearchOption.AllDirectories);

            foreach (var file in domainFiles)
            {
                var content = File.ReadAllText(file);
                if (content.Contains("using UnityEngine"))
                    errors.Add($"[Domain違反] {file} に UnityEngine 依存があります");
                if (content.Contains("using System.IO"))
                    errors.Add($"[Domain違反] {file} に System.IO 依存があります");
            }

            if (errors.Count == 0)
                Debug.Log("[ArchitectureValidator] 全チェック通過 ✅");
            else
                foreach (var e in errors)
                    Debug.LogError(e);
        }
    }
}