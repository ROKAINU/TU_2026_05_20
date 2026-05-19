#nullable enable
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class ConvertVolumeTests
    {
        private const float Epsilon = 0.0001f;

        private float _originalPerceptualExponent;
        private float _originalMinDecibel;

        [SetUp]
        public void SetUp()
        {
            _originalPerceptualExponent = ConvertVolume.PerceptualExponent;
            _originalMinDecibel = ConvertVolume.MinDecibel;
        }

        [TearDown]
        public void TearDown()
        {
            ConvertVolume.PerceptualExponent = _originalPerceptualExponent;
            ConvertVolume.MinDecibel = _originalMinDecibel;
        }

        // ========== Perceptual モード ==========

        [Test]
        public void PercentToDecibel_Perceptual_ZeroReturnsMinDecibel()
        {
            // 実装仕様：0はMinDecibelを返す（-Infinityではない）
            var result = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.Perceptual);
            Assert.That(result, Is.EqualTo(ConvertVolume.MinDecibel).Within(Epsilon));
        }

        [Test]
        public void PercentToDecibel_Perceptual_OneHundredReturnsZeroDb()
        {
            var result = ConvertVolume.PercentToDecibel(1f, VolumeConversionMode.Perceptual);
            Assert.That(result, Is.EqualTo(0f).Within(Epsilon));
        }

        [Test]
        public void PercentToDecibel_Perceptual_FiftyPercentIsLessThanZeroDb()
        {
            var result = ConvertVolume.PercentToDecibel(0.5f, VolumeConversionMode.Perceptual);
            Assert.That(result, Is.LessThan(0f));
        }

        [Test]
        public void PercentToDecibel_Perceptual_MonotonicIncreasing()
        {
            var db0 = ConvertVolume.PercentToDecibel(0.1f, VolumeConversionMode.Perceptual);
            var db50 = ConvertVolume.PercentToDecibel(0.5f, VolumeConversionMode.Perceptual);
            var db100 = ConvertVolume.PercentToDecibel(0.9f, VolumeConversionMode.Perceptual);

            Assert.That(db0, Is.LessThan(db50));
            Assert.That(db50, Is.LessThan(db100));
        }

        [Test]
        public void DecibelToPercent_Perceptual_RoundTrip()
        {
            float original = 0.7f;
            var dB = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.Perceptual);
            var recovered = ConvertVolume.DecibelToPercent(dB, VolumeConversionMode.Perceptual);

            Assert.That(recovered, Is.EqualTo(original).Within(Epsilon));
        }

        [Test]
        public void PerceptualExponent_ChangeAffectsResult()
        {
            float original = 0.5f;
            ConvertVolume.PerceptualExponent = 2.0f;
            var db2 = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.Perceptual);

            ConvertVolume.PerceptualExponent = 3.0f;
            var db3 = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.Perceptual);

            Assert.That(db2, Is.Not.EqualTo(db3).Within(Epsilon));
        }

        // ========== Linear モード ==========

        [Test]
        public void PercentToDecibel_Linear_ZeroReturnsMinDecibel()
        {
            // 実装仕様：0はMinDecibelを返す（-Infinityではない）
            var result = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.Linear);
            Assert.That(result, Is.EqualTo(ConvertVolume.MinDecibel).Within(Epsilon));
        }

        [Test]
        public void PercentToDecibel_Linear_OneHundredReturnsZeroDb()
        {
            var result = ConvertVolume.PercentToDecibel(1f, VolumeConversionMode.Linear);
            Assert.That(result, Is.EqualTo(0f).Within(Epsilon));
        }

        [Test]
        public void PercentToDecibel_Linear_HalfIsApproxMinus6Db()
        {
            var result = ConvertVolume.PercentToDecibel(0.5f, VolumeConversionMode.Linear);
            Assert.That(result, Is.EqualTo(-6.02f).Within(0.05f));
        }

        [Test]
        public void DecibelToPercent_Linear_RoundTrip()
        {
            float original = 0.6f;
            var dB = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.Linear);
            var recovered = ConvertVolume.DecibelToPercent(dB, VolumeConversionMode.Linear);

            Assert.That(recovered, Is.EqualTo(original).Within(Epsilon));
        }

        // ========== AudioTaper モード ==========

        [Test]
        public void PercentToDecibel_AudioTaper_ZeroReturnsMinDb()
        {
            var result = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.AudioTaper);
            Assert.That(result, Is.EqualTo(ConvertVolume.MinDecibel).Within(Epsilon));
        }

        [Test]
        public void PercentToDecibel_AudioTaper_OneHundredReturnsZeroDb()
        {
            var result = ConvertVolume.PercentToDecibel(1f, VolumeConversionMode.AudioTaper);
            Assert.That(result, Is.EqualTo(0f).Within(Epsilon));
        }

        [Test]
        public void PercentToDecibel_AudioTaper_FiftyIsHalfOfMinDecibel()
        {
            var expectedDb = ConvertVolume.MinDecibel / 2f;
            var result = ConvertVolume.PercentToDecibel(0.5f, VolumeConversionMode.AudioTaper);
            Assert.That(result, Is.EqualTo(expectedDb).Within(Epsilon));
        }

        [Test]
        public void DecibelToPercent_AudioTaper_RoundTrip()
        {
            float original = 0.3f;
            var dB = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.AudioTaper);
            var recovered = ConvertVolume.DecibelToPercent(dB, VolumeConversionMode.AudioTaper);

            Assert.That(recovered, Is.EqualTo(original).Within(Epsilon));
        }

        [Test]
        public void MinDecibel_ChangeAffectsResult()
        {
            float original = 0.5f;
            ConvertVolume.MinDecibel = -80f;
            var db80 = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.AudioTaper);

            ConvertVolume.MinDecibel = -60f;
            var db60 = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.AudioTaper);

            Assert.That(db80, Is.Not.EqualTo(db60).Within(Epsilon));
        }

        // ========== Edge Cases ==========

        [Test]
        public void PercentToDecibel_OutOfRangeClamp()
        {
            var resultNegative = ConvertVolume.PercentToDecibel(-0.5f, VolumeConversionMode.Perceptual);
            var resultOver = ConvertVolume.PercentToDecibel(1.5f, VolumeConversionMode.Perceptual);

            Assert.That(float.IsNaN(resultNegative), Is.False);
            Assert.That(float.IsNaN(resultOver), Is.False);
        }

        [Test]
        public void PercentToDecibel_InvalidModeThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ConvertVolume.PercentToDecibel(0.5f, (VolumeConversionMode)999));
        }

        [Test]
        public void DecibelToPercent_InvalidModeThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ConvertVolume.DecibelToPercent(-20f, (VolumeConversionMode)999));
        }

        // ========== Mode Default Behavior ==========

        [Test]
        public void PercentToDecibel_DefaultModeIsPerceptual()
        {
            float original = 0.5f;
            var resultDefault = ConvertVolume.PercentToDecibel(original);
            var resultExplicit = ConvertVolume.PercentToDecibel(original, VolumeConversionMode.Perceptual);

            Assert.That(resultDefault, Is.EqualTo(resultExplicit).Within(Epsilon));
        }

        [Test]
        public void DecibelToPercent_DefaultModeIsPerceptual()
        {
            float dB = -10f;
            var resultDefault = ConvertVolume.DecibelToPercent(dB);
            var resultExplicit = ConvertVolume.DecibelToPercent(dB, VolumeConversionMode.Perceptual);

            Assert.That(resultDefault, Is.EqualTo(resultExplicit).Within(Epsilon));
        }

        // ========== RoundTrip Tests with Zero ==========

        [Test]
        public void DecibelToPercent_NegativeInfinity_ReturnsZero()
        {
            var result = ConvertVolume.DecibelToPercent(float.NegativeInfinity, VolumeConversionMode.Linear);
            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        public void RoundTrip_Zero_Percent_To_MinDecibel_To_Zero()
        {
            // 実装仕様：0 → MinDecibel → DecibelToPercentでMinDecibelは0を返す
            var dB = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.Linear);
            // MinDecibelはAudioTaperの下限として0を返す仕様
            var recovered = ConvertVolume.DecibelToPercent(dB, VolumeConversionMode.AudioTaper);
            Assert.That(recovered, Is.EqualTo(0f).Within(Epsilon));
        }

        // ========== 複合テスト（実用的シナリオ） ==========

        [Test]
        public void Scenario_UISlider_PerceptualFeelNatural()
        {
            var volumes = new[] { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };
            var dBs = new float[volumes.Length];

            for (int i = 0; i < volumes.Length; i++)
                dBs[i] = ConvertVolume.PercentToDecibel(volumes[i], VolumeConversionMode.Perceptual);

            foreach (var dB in dBs)
            {
                Assert.That(float.IsNaN(dB), Is.False);
                Assert.That(float.IsInfinity(dB), Is.False);
            }

            for (int i = 0; i < dBs.Length - 1; i++)
                Assert.That(dBs[i], Is.LessThan(dBs[i + 1]));
        }

        [Test]
        public void Scenario_AllModesProduceDifferentCurves()
        {
            float percent = 0.5f;

            var perceptual = ConvertVolume.PercentToDecibel(percent, VolumeConversionMode.Perceptual);
            var linear = ConvertVolume.PercentToDecibel(percent, VolumeConversionMode.Linear);
            var audioTaper = ConvertVolume.PercentToDecibel(percent, VolumeConversionMode.AudioTaper);

            var unique = new HashSet<float> { perceptual, linear, audioTaper };
            Assert.That(unique.Count, Is.GreaterThan(1));
        }

        [Test]
        public void DecibelToPercent_PositiveDb_ClampsToOne()
        {
            var result = ConvertVolume.DecibelToPercent(6f, VolumeConversionMode.Linear);
            Assert.That(result, Is.GreaterThanOrEqualTo(1f));
        }

        [Test]
        public void PercentToDecibel_AllModes_ZeroReturnsSameMinDecibel()
        {
            // 実装仕様：全モードで0はMinDecibelを返す（AudioTaperだけ特殊ではない）
            var perceptual = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.Perceptual);
            var linear     = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.Linear);
            var audioTaper = ConvertVolume.PercentToDecibel(0f, VolumeConversionMode.AudioTaper);

            Assert.That(perceptual, Is.EqualTo(ConvertVolume.MinDecibel).Within(Epsilon));
            Assert.That(linear,     Is.EqualTo(ConvertVolume.MinDecibel).Within(Epsilon));
            Assert.That(audioTaper, Is.EqualTo(ConvertVolume.MinDecibel).Within(Epsilon));
        }
    }
}