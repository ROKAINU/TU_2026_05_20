#nullable enable
using NUnit.Framework;

namespace Game.Domain.Tests
{
    public class SettingSaveDataTests
    {
        // -----------------------------------------------------------------------
        // Constructor
        // -----------------------------------------------------------------------

        [Test]
        public void Constructor_SetsValuesCorrectly()
        {
            var data = new SettingSaveData(
                bgmVolume: 0.8f,
                seVolume: 0.3f);

            Assert.That(data.BGMVolume, Is.EqualTo(0.8f));
            Assert.That(data.SEVolume, Is.EqualTo(0.3f));
        }

        // -----------------------------------------------------------------------
        // Default
        // -----------------------------------------------------------------------

        [Test]
        public void Default_ReturnsExpectedValues()
        {
            var data = SettingSaveData.Default();

            Assert.That(data.BGMVolume, Is.EqualTo(0.5f));
            Assert.That(data.SEVolume, Is.EqualTo(0.5f));
        }

        // -----------------------------------------------------------------------
        // With
        // -----------------------------------------------------------------------

        [Test]
        public void With_ChangesOnlyBGMVolume()
        {
            var original = new SettingSaveData(
                bgmVolume: 0.5f,
                seVolume: 0.2f);

            var updated = original.With(bgmVolume: 0.9f);

            Assert.That(updated.BGMVolume, Is.EqualTo(0.9f));
            Assert.That(updated.SEVolume, Is.EqualTo(0.2f));
        }

        [Test]
        public void With_ChangesOnlySEVolume()
        {
            var original = new SettingSaveData(
                bgmVolume: 0.5f,
                seVolume: 0.2f);

            var updated = original.With(seVolume: 0.9f);

            Assert.That(updated.BGMVolume, Is.EqualTo(0.5f));
            Assert.That(updated.SEVolume, Is.EqualTo(0.9f));
        }

        [Test]
        public void With_ChangesBothValues()
        {
            var original = new SettingSaveData(
                bgmVolume: 0.1f,
                seVolume: 0.2f);

            var updated = original.With(
                bgmVolume: 0.7f,
                seVolume: 0.8f);

            Assert.That(updated.BGMVolume, Is.EqualTo(0.7f));
            Assert.That(updated.SEVolume, Is.EqualTo(0.8f));
        }

        [Test]
        public void With_NoArguments_ReturnsSameValues()
        {
            var original = new SettingSaveData(
                bgmVolume: 0.4f,
                seVolume: 0.6f);

            var updated = original.With();

            Assert.That(updated.BGMVolume, Is.EqualTo(original.BGMVolume));
            Assert.That(updated.SEVolume, Is.EqualTo(original.SEVolume));
        }

        // -----------------------------------------------------------------------
        // Immutability
        // -----------------------------------------------------------------------

        [Test]
        public void With_DoesNotMutateOriginal()
        {
            var original = new SettingSaveData(
                bgmVolume: 0.2f,
                seVolume: 0.3f);

            var updated = original.With(bgmVolume: 1.0f);

            Assert.That(original.BGMVolume, Is.EqualTo(0.2f));
            Assert.That(original.SEVolume, Is.EqualTo(0.3f));

            Assert.That(updated.BGMVolume, Is.EqualTo(1.0f));
            Assert.That(updated.SEVolume, Is.EqualTo(0.3f));
        }
    }
}