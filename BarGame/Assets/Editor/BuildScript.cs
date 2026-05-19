#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor
{
    public static class BuildScript
    {
        [MenuItem("Build/Windows")]
        public static void BuildWindows()
        {
            Build(BuildTarget.StandaloneWindows64, "Builds/Windows/Game.exe");
        }

        [MenuItem("Build/Android")]
        public static void BuildAndroid()
        {
            Build(BuildTarget.Android, "Builds/Android/Game.apk");
        }

        private static void Build(
            BuildTarget target,
            string outputPath)
        {
            var scenes = GetEnabledScenes();

            if (scenes.Length == 0)
                throw new Exception("No enabled scenes in Build Settings.");

            Directory.CreateDirectory(
                Path.GetDirectoryName(outputPath)!);

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };

            BuildReport report =
                BuildPipeline.BuildPlayer(options);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build Success: {report.summary.outputPath}");
            }
            else
            {
                Debug.LogError($"Build Failed: {report.summary.result}");
                throw new Exception($"Build failed: {report.summary.result}");
            }
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = new List<string>();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    scenes.Add(scene.path);
            }

            return scenes.ToArray();
        }
    }
}

#endif