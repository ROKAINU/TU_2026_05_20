#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Presentation.View
{
    public class QuitGame : MonoBehaviour
    {
        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; // エディターの再生停止
#else
            Application.Quit(); // ビルド実行時
#endif
        }
    }
}