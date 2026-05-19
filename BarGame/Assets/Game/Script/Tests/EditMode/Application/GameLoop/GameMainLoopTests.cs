#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using NUnit.Framework;
using Game.Application;
using Game.Application.Contracts;
using Game.Domain;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;
using Game.Kernel.Utils.R3;

namespace Game.Application.Tests
{
    public class GameMainLoopTests
    {
        // -----------------------------------------------------------------------
        // RunAsync
        // -----------------------------------------------------------------------

        [Test]
        public async Task RunAsync_WhenCanceled_ReturnsCanceled()
        {
            var loop = CreateLoop();

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await loop.RunAsync(cts.Token);

            Assert.That(result, Is.EqualTo(GameMainLoopResult.Canceled));
        }

        [Test]
        public async Task RunAsync_WhenTimeUp_ReturnsCleared()
        {
            var gameMainStore = new Store<GameMainState>(
                new GameMainState(
                    remainingTime: new RemainingTime(0f)));

            var loop = CreateLoop(gameMainStore);

            var result = await loop.RunAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(GameMainLoopResult.Cleared));
        }

        [Test]
        public async Task RunAsync_PauseCommand_TransitionsToPaused()
        {
            var queue = new FakeAsyncCommandQueue();

            queue.Enqueue(new GameCommand(GameCommandType.Pause));

            var pausePublisher = new FakePausePublisher();

            var loop = CreateLoop(
                input: queue,
                pausePublisher: pausePublisher);

            using var cts = new CancellationTokenSource();

            // 無限ループ防止
            cts.CancelAfterSlim(100);

            await loop.RunAsync(cts.Token);

            Assert.That(
                pausePublisher.PublishedMessages.Count,
                Is.GreaterThan(0));

            Assert.That(
                pausePublisher.PublishedMessages[0].IsPausing,
                Is.True);
        }

        // -----------------------------------------------------------------------
        // Factory
        // -----------------------------------------------------------------------

        private static GameMainLoop CreateLoop(
            Store<GameMainState>? gameMainStore = null,
            IAsyncCommandQueue<GameCommand>? input = null,
            FakePausePublisher? pausePublisher = null)
        {
            gameMainStore ??= new Store<GameMainState>(
                new GameMainState(
                    remainingTime: new RemainingTime(100f)));

            var globalStore = new Store<GameGlobalState>(
                new GameGlobalState());

            input ??= new FakeAsyncCommandQueue();

            pausePublisher ??= new FakePausePublisher();

            var handler = new FakeCommandHandler();

            var playing = new PlayingPhase(
                gameMainStore,
                globalStore,
                input,
                handler,
                pausePublisher);

            var paused = new PausedPhase(
                input,
                pausePublisher);

            return new GameMainLoop(
                playing,
                paused,
                new FakeGameTime());
        }

        // -----------------------------------------------------------------------
        // Fake
        // -----------------------------------------------------------------------

        private sealed class FakeGameTime : IGameTime
        {
            public float DeltaTime => 0.016f;

            public float Now => 0f;

            public float RealtimeSinceStartup => 0f;
        }

        private sealed class FakeCommandHandler
            : ICommandHandler<GameCommand>
        {
            public UniTask HandleAsync(
                GameCommand command,
                CancellationToken cancellationToken)
            {
                return UniTask.CompletedTask;
            }
        }

        private sealed class FakeAsyncCommandQueue
            : IAsyncCommandQueue<GameCommand>
        {
            private readonly Queue<GameCommand> _queue = new();

            public void Enqueue(in GameCommand command)
            {
                _queue.Enqueue(command);
            }

            public bool TryDequeue(out GameCommand command)
            {
                if (_queue.Count > 0)
                {
                    command = _queue.Dequeue();
                    return true;
                }

                command = default;
                return false;
            }

            public UniTask<GameCommand> NextAsync(CancellationToken token)
            {
                if (_queue.Count > 0)
                    return UniTask.FromResult(_queue.Dequeue());

                return UniTask.FromCanceled<GameCommand>(token);
            }

            public void Clear()
            {
                _queue.Clear();
            }
        }

        private sealed class FakePausePublisher
            : IAsyncPublisher<GamePauseMessage>
        {
            public readonly List<GamePauseMessage> PublishedMessages = new();

            // 同期Publish
            public void Publish(
                GamePauseMessage message,
                CancellationToken cancellationToken = default)
            {
                PublishedMessages.Add(message);
            }

            // 非同期Publish
            public UniTask PublishAsync(
                GamePauseMessage message,
                CancellationToken cancellationToken = default)
            {
                PublishedMessages.Add(message);
                return UniTask.CompletedTask;
            }

            // AsyncPublishStrategy付き
            public UniTask PublishAsync(
                GamePauseMessage message,
                AsyncPublishStrategy strategy,
                CancellationToken cancellationToken = default)
            {
                PublishedMessages.Add(message);
                return UniTask.CompletedTask;
            }
        }
    }
}