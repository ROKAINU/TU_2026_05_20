using UnityEngine;
using Game.Application;
using Game.Presentation.View;
using Game.Kernel.Utils.Cysharp;

namespace Game.Presentation
{
    public class GameMainCommandBinder
    {
        private readonly IAsyncCommandQueue<GameCommand> _commandQueue;
        private readonly GameMainUIInstance _uiInstance;

        internal GameMainCommandBinder(IAsyncCommandQueue<GameCommand> queue, GameMainUIInstance uiInstance)
        {
            _commandQueue = queue;
            _uiInstance = uiInstance;
        }

        public void Bind()
        {
            if (_uiInstance.pauseButton != null)
                _uiInstance.pauseButton.onClick.AddListener(() =>
                    {
                        _commandQueue.Enqueue(new GameCommand(GameCommandType.Pause));
                    });

            if (_uiInstance.resumeButton != null)
                _uiInstance.resumeButton.onClick.AddListener(() =>
                    {
                        _commandQueue.Enqueue(new GameCommand(GameCommandType.Resume));
                    });
        }
    }
}
