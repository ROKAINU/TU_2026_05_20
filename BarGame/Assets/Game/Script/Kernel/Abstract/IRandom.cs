#nullable enable

namespace Game.Kernel
{
    /// <summary>
    /// 乱数抽象。再現性（リプレイ/テスト）目的で差し替え可能にする。
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// [minInclusive, maxExclusive) の範囲で int を返す。
        /// UnityEngine.Random.Range(int,int) と同じ契約。
        /// </summary>
        int Range(int minInclusive, int maxExclusive);

        /// <summary>
        /// [minInclusive, maxInclusive] の範囲で float を返す。
        /// UnityEngine.Random.Range(float,float) と同じ契約。
        /// </summary>
        float Range(float minInclusive, float maxInclusive);

        /// <summary>
        /// 0〜1 の乱数を返す。UnityEngine.Random.value 相当。
        /// 注意: Unity の value は 1.0 も返し得る（仕様上は [0.0, 1.0]）。
        /// </summary>
        float Value01 { get; }
    }
}