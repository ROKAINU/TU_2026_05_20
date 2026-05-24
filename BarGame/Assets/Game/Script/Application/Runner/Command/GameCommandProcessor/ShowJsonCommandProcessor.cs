using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Kernel;
using Game.Application.Contracts;

namespace Game.Application.Runner
{
    /// <summary>
    /// ShowJson コマンド処理
    /// </summary>
    public sealed class ShowJsonCommandProcessor : IGameCommandProcessor
    {
        private readonly IMasterDataRepository _repository;
        private readonly LoggerBase _logger;

        public ShowJsonCommandProcessor(
            IMasterDataRepository repository,
            LoggerBase logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public bool CanProcess(GameCommand command)
            => command is GameCommand.ShowJson;

        public UniTask ProcessAsync(GameCommand command, CancellationToken token)
        {
            if (command is not GameCommand.ShowJson showJson)
                return UniTask.CompletedTask;

            if (showJson.Id <= 0)
            {
                _logger.LogWarning($"Invalid JSON ID: {showJson.Id}");
                return UniTask.CompletedTask;
            }

            var characters = _repository.Load<CharacterMasterRecord>(
                CharacterMasterRecord.FilePath);
                
            var target = characters.Find(x => x.id == showJson.Id);
            if (target == null)
            {
                _logger.LogWarning($"Character not found. id={showJson.Id}");
                return UniTask.CompletedTask;
            }

            _logger.LogInfo(
                $"Character Found : " +
                $"id={target.id}, name={target.name}, " +
                $"level={target.level}, hp={target.hp}, " +
                $"attack={target.attack}, active={target.active}");

            return UniTask.CompletedTask;
        }
    }
}