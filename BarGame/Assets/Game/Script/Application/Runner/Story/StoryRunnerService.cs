using System;
using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Application.Contracts;
using Game.Application.Contracts.Cysharp;

namespace Game.Application.Runner
{
    /// <summary>
    /// ストーリー実行の進行制御（pure C#）
    /// Ink, Unity依存なし
    /// </summary>
    public sealed class StoryRunnerService : IStoryRunner
    {
        public async UniTask RunScenarioAsync(
            IScenario scenario,
            IGameStateContainer gameState,
            IStoryDisplay display,
            IAdvanceInput input,
            CancellationToken cancellationToken)
        {
            await RunMainLoopAsync(scenario, display, input, cancellationToken);
        }
 
        private async UniTask RunMainLoopAsync(
            IScenario scenario,
            IStoryDisplay display,
            IAdvanceInput input,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
 
                // テキスト表示フェーズ
                await DisplayTextPhaseAsync(scenario, display, input, cancellationToken);
 
                // 選択肢チェック
                if (!scenario.HasChoices)
                    return;
 
                // 選択肢フェーズ
                int selectedIndex = await DisplayChoicesPhaseAsync(
                    scenario, display, cancellationToken);
 
                scenario.SelectChoice(selectedIndex);
            }
        }
 
        private async UniTask DisplayTextPhaseAsync(
            IScenario scenario,
            IStoryDisplay display,
            IAdvanceInput input,
            CancellationToken cancellationToken)
        {
            while (scenario.CanContinue)
            {
                cancellationToken.ThrowIfCancellationRequested();
 
                var line = scenario.GetNextLine();
                if (!string.IsNullOrEmpty(line))
                {
                    display.DisplayText(line);
                }
 
                // 入力 or タイムアウトまで待機
                await WaitForAdvanceAsync(input, cancellationToken);
            }
        }
 
        private async UniTask<int> DisplayChoicesPhaseAsync(
            IScenario scenario,
            IStoryDisplay display,
            CancellationToken cancellationToken)
        {
            var choices = scenario.GetChoices();
            var choiceTcs = new UniTaskCompletionSource<int>();
 
            display.DisplayChoices(choices.ToArray(), index =>
            {
                choiceTcs.TrySetResult(index);
            });
 
            cancellationToken.Register(() =>
            {
                choiceTcs.TrySetCanceled(cancellationToken);
            });
 
            int selected = await choiceTcs.Task;
            display.ClearChoices();
            return selected;
        }
 
        private async UniTask WaitForAdvanceAsync(IAdvanceInput input, CancellationToken cancellationToken)
        {
            // タイムアウト（500ms）
            var timeoutTask = UniTask.Delay(500, cancellationToken: cancellationToken);
 
            // 入力待機
            var inputTask = WaitForInputAsync(input, cancellationToken);
 
            // どちらか先に終わったら進む
            await UniTask.WhenAny(timeoutTask, inputTask);
        }
 
        private async UniTask WaitForInputAsync(IAdvanceInput input, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (input.IsAdvancePressed())
                    return;
 
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}