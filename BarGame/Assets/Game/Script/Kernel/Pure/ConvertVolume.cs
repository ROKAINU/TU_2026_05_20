using System;

namespace Game.Kernel
{
    /// <summary>
    /// ボリューム変換の方式
    /// </summary>
    public enum VolumeConversionMode
    {
        /// <summary>
        /// 人の聴覚特性に合わせた知覚的変換（xのk乗をLinear変換）。
        /// UIスライダーなど、ユーザーが自然に感じる音量変化に向いている。
        /// </summary>
        Perceptual,

        /// <summary>
        /// 振幅比をそのままdBに変換（20*log10）。
        /// AudioMixerやDSPなど技術的な用途に正確。
        /// </summary>
        Linear,

        /// <summary>
        /// dBスケール上で線形にマッピング（AudioTaper）。
        /// dB値が均等に動く音量ノブ的なUIに向いている。
        /// </summary>
        AudioTaper,
    }

    public static class ConvertVolume
    {
        // --- 設定可能な定数 ---

        /// <summary>ゼロ除算・log(0)を防ぐ最小値</summary>
        private const float Epsilon = 0.0001f;

        /// <summary>Perceptualモードの指数（2〜3推奨。大きいほど低音量域が広がる）</summary>
        public static float PerceptualExponent { get; set; } = 2.0f;

        /// <summary>最小dB値（無音相当）</summary>
        public static float MinDecibel { get; set; } = -80f;

        // --- Public API ---

        /// <summary>
        /// 0.0〜1.0のボリューム値をdBに変換する。
        /// </summary>
        public static float PercentToDecibel(float value, VolumeConversionMode mode = VolumeConversionMode.Perceptual)
        {
            // 0に近い値は-80dB（無音）にする。AudioTaperモードではMinDecibel以上を保証する。
            if (value <= 0.001f) // ほぼ0の場合
            {
                return MinDecibel; // -80dB（無音）
            }

            value = Clamp(value, Epsilon, 1f);

            return mode switch
            {
                VolumeConversionMode.Linear      => LinearToDb(value),
                VolumeConversionMode.Perceptual  => LinearToDb(MathF.Pow(value, PerceptualExponent)),
                VolumeConversionMode.AudioTaper  => AudioTaperToDb(value),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
            };
        }

        /// <summary>
        /// dBを0.0〜1.0のボリューム値に変換する。
        /// </summary>
        public static float DecibelToPercent(float dB, VolumeConversionMode mode = VolumeConversionMode.Perceptual)
        {
            // -∞ や MinDecibel は 0 に変換
            if (float.IsNegativeInfinity(dB) || (mode == VolumeConversionMode.AudioTaper && dB <= MinDecibel))
            {
                return 0f;
            }

            return mode switch
            {
                VolumeConversionMode.Linear      => DbToLinear(dB),
                VolumeConversionMode.Perceptual  => MathF.Pow(DbToLinear(dB), 1f / PerceptualExponent),
                VolumeConversionMode.AudioTaper  => DbToAudioTaper(dB),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
            };
        }

        // --- 内部実装 ---

        // Linear: 振幅比 → dB（20*log10）
        private static float LinearToDb(float linear)
            => 20f * MathF.Log10(linear);

        // Linear: dB → 振幅比
        private static float DbToLinear(float dB)
            => MathF.Pow(10f, dB / 20f);

        // AudioTaper: 0〜1 → minDb〜0dB の線形マッピング
        private static float AudioTaperToDb(float value)
            => MinDecibel + (0f - MinDecibel) * value;

        // AudioTaper: dB → 0〜1
        private static float DbToAudioTaper(float dB)
            => Clamp((dB - MinDecibel) / (0f - MinDecibel), 0f, 1f);

        // System.Math.Clamp は .NET Standard 2.1 以上が必要なため自前実装
        private static float Clamp(float value, float min, float max)
            => value < min ? min : value > max ? max : value;
    }
}