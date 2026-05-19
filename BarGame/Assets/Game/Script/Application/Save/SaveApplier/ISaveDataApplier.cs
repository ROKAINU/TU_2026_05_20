using Cysharp.Threading.Tasks;
using Game.Domain;

namespace Game.Application
{
    public interface ISaveDataApplier
    {
        /// <summary>
        /// ゲームのセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用するゲームのセーブデータ</param>
        UniTask ApplyGameDataToStoresAsync(GameSaveData saveData);

        /// <summary>
        /// 設定のセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用する設定のセーブデータ</param>
        UniTask ApplySettingsDataToStoresAsync(SettingSaveData saveData);

        /// <summary>
        /// ゲームのセーブデータと設定のセーブデータをストアに適用する。セーブデータの内容に基づいて、ストアの状態を更新する。
        /// </summary>
        /// <param name="saveData">適用するゲームのセーブデータ</param>
        /// <param name="settingData">適用する設定のセーブデータ</param>
        UniTask ApplyAllDataToStoresAsync(GameSaveData saveData, SettingSaveData settingData);
    }
}