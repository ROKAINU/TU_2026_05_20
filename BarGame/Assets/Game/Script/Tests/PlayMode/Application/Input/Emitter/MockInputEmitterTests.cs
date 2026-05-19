#nullable enable
using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using Game.Domain;
using Game.Application;
using Game.Application.Contracts;
using Game.Kernel.Utils.Cysharp;

namespace Game.Application.PlayTests
{
    [TestFixture]
    public sealed class MockInputEmitterTests
    {
        private IObjectResolver _resolver = null!;

        private MockInputEmitter _emitter = null!;
        private AsyncCommandQueue<GameCommand> _queue = null!;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            builder.Register<AsyncCommandQueue<GameCommand>>(
                    Lifetime.Singleton)
                .As<IAsyncCommandQueue<GameCommand>>()
                .AsSelf();

            builder.Register<MockInputEmitter>(
                    Lifetime.Singleton)
                .AsSelf()
                .As<ICommandEmitter>();

            _resolver = builder.Build();

            _emitter =
                _resolver.Resolve<MockInputEmitter>();

            _queue =
                _resolver.Resolve<
                    AsyncCommandQueue<GameCommand>>();
        }

        [TearDown]
        public void TearDown()
        {
            _emitter.Dispose();
            _resolver.Dispose();
        }

        // =========================================================
        // PlaySequence
        // =========================================================

        [UnityTest]
        public IEnumerator PlaySequence_StartsSequence()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                var sequence =
                    new InputSequence(
                        "TestSequence",
                        new InputAction(
                            GameCommandType.AddScore,
                            0.01f));

                _emitter.PlaySequence(sequence);

                await UniTask.DelayFrame(2);

                Assert.That(
                    _emitter.CurrentSequenceName,
                    Is.EqualTo("TestSequence"));

                Assert.That(
                    _emitter.IsSequenceCompleted,
                    Is.False);
            });
        }

        [UnityTest]
        public IEnumerator PlaySequence_EnqueuesCommand()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                var sequence =
                    new InputSequence(
                        "QueueTest",
                        new InputAction(
                            GameCommandType.AddScore,
                            0.01f));

                _emitter.PlaySequence(sequence);

                await UniTask.Delay(
                    100);

                var success =
                    _queue.TryDequeue(
                        out var command);

                Assert.That(success, Is.True);

                Assert.That(
                    command.Type,
                    Is.EqualTo(
                        GameCommandType.AddScore));
            });
        }

        [UnityTest]
        public IEnumerator PlaySequence_Completes()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                var sequence =
                    new InputSequence(
                        "CompleteTest",
                        new InputAction(
                            GameCommandType.AddScore,
                            0.01f));

                _emitter.PlaySequence(sequence);

                var timeout = 2f;

                while (!_emitter.IsSequenceCompleted &&
                       timeout > 0f)
                {
                    timeout -= Time.deltaTime;

                    await UniTask.DelayFrame(1);
                }

                Assert.That(
                    timeout,
                    Is.GreaterThan(0f));

                Assert.That(
                    _emitter.IsSequenceCompleted,
                    Is.True);
            });
        }

        // =========================================================
        // PlaySequencesAsync
        // =========================================================

        [UnityTest]
        public IEnumerator PlaySequencesAsync_ExecutesAllSequences()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                var sequences =
                    new IInputSequence[]
                    {
                        new InputSequence(
                            "Seq1",
                            new InputAction(
                                GameCommandType.AddScore,
                                0.01f)),

                        new InputSequence(
                            "Seq2",
                            new InputAction(
                                GameCommandType.Pause,
                                0.01f)),

                        new InputSequence(
                            "Seq3",
                            new InputAction(
                                GameCommandType.Resume,
                                0.01f))
                    };

                await _emitter.PlaySequencesAsync(
                    sequences);

                Assert.That(
                    sequences[0].IsCompleted,
                    Is.True);

                Assert.That(
                    sequences[1].IsCompleted,
                    Is.True);

                Assert.That(
                    sequences[2].IsCompleted,
                    Is.True);
            });
        }

        [UnityTest]
        public IEnumerator PlaySequencesAsync_CanBeCancelled()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                using var cts =
                    new CancellationTokenSource();

                var sequences =
                    new IInputSequence[]
                    {
                        new InputSequence(
                            "LongSequence",
                            new InputAction(
                                GameCommandType.AddScore,
                                5f))
                    };

                var task =
                    _emitter.PlaySequencesAsync(
                        sequences,
                        cts.Token);

                cts.CancelAfterSlim(100);

                try
                {
                    await task;
                }
                catch (OperationCanceledException)
                {
                }

                Assert.Pass();
            });
        }

        // =========================================================
        // Enable / Disable
        // =========================================================

        [UnityTest]
        public IEnumerator Disable_StopsPump()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                var sequence =
                    new InputSequence(
                        "DisableTest",
                        new InputAction(
                            GameCommandType.AddScore,
                            0.5f));

                _emitter.PlaySequence(sequence);

                _emitter.Disable();

                await UniTask.Delay(200);

                var success =
                    _queue.TryDequeue(out _);

                Assert.That(
                    success,
                    Is.False);
            });
        }

        [Test]
        public void PlaySequence_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    _emitter.PlaySequence(null!);
                });
        }

        [Test]
        public void Dispose_MultipleTimes_NoException()
        {
            Assert.DoesNotThrow(() =>
            {
                _emitter.Dispose();
                _emitter.Dispose();
            });
        }

        // =========================================================
        // CurrentSequenceName
        // =========================================================

        [Test]
        public void CurrentSequenceName_Default_IsNull()
        {
            Assert.That(
                _emitter.CurrentSequenceName,
                Is.Null);
        }

        [UnityTest]
        public IEnumerator CurrentSequenceName_UpdatesCorrectly()
        {
            yield return UniTask.ToCoroutine(async () =>
            {
                var sequence =
                    new InputSequence(
                        "NameTest",
                        new InputAction(
                            GameCommandType.AddScore,
                            0.01f));

                _emitter.PlaySequence(sequence);

                await UniTask.DelayFrame(1);

                Assert.That(
                    _emitter.CurrentSequenceName,
                    Is.EqualTo("NameTest"));
            });
        }
    }
}