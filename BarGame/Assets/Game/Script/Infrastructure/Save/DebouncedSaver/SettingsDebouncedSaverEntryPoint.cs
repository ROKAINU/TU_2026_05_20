using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Game.Infrastructure.Save
{
    public sealed class SettingsDebouncedSaverEntryPoint : IAsyncStartable
    {
        private readonly SettingsDebouncedSaver _saver;

        public SettingsDebouncedSaverEntryPoint(SettingsDebouncedSaver saver)
        {
            _saver = saver;
        }

        /// <summary>
        /// 設定の変更を監視し、変更があった場合に一定時間待ってから保存する。変更が頻繁にある場合は、最後の変更から一定時間経過してから保存される。
        /// </summary>
        /// <param name="ct">キャンセル用のトークン</param>
        /// <returns></returns>
        public UniTask StartAsync(CancellationToken ct)
        {
            _saver.Start();
            return UniTask.CompletedTask;
        }
    }
}