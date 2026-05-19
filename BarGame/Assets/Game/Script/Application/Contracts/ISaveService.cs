using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;

namespace Game.Application.Contracts
{
    public interface ISaveService
    {
        /// <summary>
        /// ゲームのセーブデータを保存する。セーブデータの内容に基づいて、ゲームの状態を永続化する。保存処理は非同期で行われ、完了まで待機できる。
        /// </summary>
        /// <param name="saveData">保存するゲームのセーブデータ</param>
        /// <param name="ct">キャンセル トークン</param>
        UniTask SaveGameDataAsync(GameSaveData saveData, CancellationToken ct = default);

        /// <summary>
        /// 設定のセーブデータを保存する。セーブデータの内容に基づいて、設定の状態を永続化する。保存処理は非同期で行われ、完了まで待機できる。
        /// </summary>
        /// <param name="saveData">保存する設定のセーブデータ</param>
        /// <param name="ct">キャンセル トークン</param>
        UniTask SaveSettingsDataAsync(SettingSaveData saveData, CancellationToken ct = default);

        /// <summary>
        /// ゲームのセーブデータと設定のセーブデータを保存する。セーブデータの内容に基づいて、ゲームの状態と設定の状態を永続化する。保存処理は非同期で行われ、完了まで待機できる。
        /// </summary>
        /// <param name="ct">キャンセル トークン</param>
        /// <returns>ゲームのセーブデータ</returns>
        UniTask<GameSaveData> LoadGameDataAsync(CancellationToken ct = default);

        /// <summary>
        /// 設定のセーブデータを読み込む。セーブデータの内容に基づいて、設定の状態を復元する。読み込み処理は非同期で行われ、完了まで待機できる。
        /// </summary>
        /// <param name="ct">キャンセル トークン</param>
        /// <returns>設定のセーブデータ</returns>
        UniTask<SettingSaveData> LoadSettingsDataAsync(CancellationToken ct = default);

        /// <summary>
        /// ゲームのセーブデータと設定のセーブデータを読み込む。セーブデータの内容に基づいて、ゲームの状態と設定の状態を復元する。読み込み処理は非同期で行われ、完了まで待機できる。
        /// </summary>
        /// <param name="ct">キャンセル トークン</param>
        UniTask InitializeAsync(CancellationToken ct = default);
    }
}