using Game.Infrastructure.Ink;
using Game.Application.Contracts;
using UnityEngine;

namespace Game.Presentation
{
 
    /// <summary>
    /// Ink外部関数とUI表示の結合
    /// ゲーム固有のストーリーロジックをここに書く
    /// </summary>
    public sealed class StorySetupPresenter
    {
        public static void SetupExternalFunctions(
            InkScenario scenario,
            IStoryDisplay display)
        {
            // SetName → UI表示
            scenario.BindExternalFunction("SetName", (string name) =>
            {
                display.DisplayName(name);
            });
 
            // SetBackground → UI表示
            scenario.BindExternalFunction("SetBackground", (string backgroundId) =>
            {
                display.DisplayBackground(backgroundId);
            });
 
            // ゲーム固有のロジック例
            scenario.BindExternalFunction("AddPoint", (string pointType, int amount) =>
            {
                Debug.Log($"{pointType} に {amount} ポイント追加");
                // ここで Redux dispatch など
            });
        }
    }
}