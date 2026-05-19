// Assets/Scripts/Core/Domain/Game/GameMessages.cs
namespace Game.Domain
{
    /// <summary>
    /// シーン遷移メッセージ
    /// </summary>
    public readonly struct TransitionSceneMessage
    {
        public SceneId TargetScene { get; }
        
        public TransitionSceneMessage(SceneId targetScene)
        {
            TargetScene = targetScene;
        }
    }
}