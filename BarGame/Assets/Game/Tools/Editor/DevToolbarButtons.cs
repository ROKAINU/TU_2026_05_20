using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Tools
{
    [InitializeOnLoad]
    public static class DevToolbarButtons
    {
        static DevToolbarButtons()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
        }

        static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            // ボタン1：PlayerPrefs クリア
            if (GUILayout.Button(
                new GUIContent("🗑 Prefs", "PlayerPrefs を全削除"),
                ToolbarStyles.commandButtonStyle))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("[DevToolbar] PlayerPrefs cleared.");
            }

            // ボタン2：Architecture Validator 実行
            if (GUILayout.Button(
                new GUIContent("✓ Arch", "Architecture Validator を実行"),
                ToolbarStyles.commandButtonStyle))
            {
                ArchitectureValidator.RunValidation();
            }

            // ボタン3：ゲームシーンから直接再生
            if (GUILayout.Button(
                new GUIContent("▶ Game", "GameScene から再生開始"),
                ToolbarStyles.commandButtonStyle))
            {
                SceneHelper.StartScene("Assets/Game/Scenes/GameScene.unity");
            }
        }
    }

    // ボタンのスタイル
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                fixedWidth = 60,
                fixedHeight = 20,
            };
        }
    }

    // シーン再生ヘルパー
    static class SceneHelper
    {
        public static void StartScene(string scenePath)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            UnityEditor.SceneManagement.EditorSceneManager
                .SaveCurrentModifiedScenesIfUserWantsTo();
            UnityEditor.SceneManagement.EditorSceneManager
                .OpenScene(scenePath);
            EditorApplication.isPlaying = true;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            }
        }
    }
}