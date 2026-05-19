#nullable enable
using NUnit.Framework;
using Game.Domain;
using Game.Infrastructure.Save;
using Game.Kernel;

namespace Game.Infrastructure.Save.Tests
{
    public class SaveMigratorTests
    {
        private SaveMigrator _migrator = null!;

        [SetUp]
        public void SetUp()
        {
            _migrator = new SaveMigrator(new NullLogger());
        }

        // -----------------------------------------------------------------------
        // AlreadyLatest
        // -----------------------------------------------------------------------

        [Test]
        public void Migrate_SameVersion_ReturnsAlreadyLatest()
        {
            var gameData = GameSaveData.Default();
            var settingsData = SettingSaveData.Default();

            var (result, _, _) = _migrator.Migrate(GameVersion.Major, gameData, settingsData);

            Assert.That(result, Is.EqualTo(SaveMigrator.MigrationResult.AlreadyLatest));
        }

        [Test]
        public void Migrate_SameVersion_DataUnchanged()
        {
            var gameData = new GameSaveData(testScore: 100, testHighScore: 200);
            var settingsData = new SettingSaveData(bgmVolume: 0.8f, seVolume: 0.6f);

            var (_, returnedGame, returnedSettings) = _migrator.Migrate(GameVersion.Major, gameData, settingsData);

            Assert.That(returnedGame.TestScore,         Is.EqualTo(100));
            Assert.That(returnedGame.TestHighScore,     Is.EqualTo(200));
            Assert.That(returnedSettings.BGMVolume,     Is.EqualTo(0.8f));
            Assert.That(returnedSettings.SEVolume,      Is.EqualTo(0.6f));
        }

        // -----------------------------------------------------------------------
        // Downgrade
        // -----------------------------------------------------------------------

        [Test]
        public void Migrate_SavedVersionNewer_ReturnsDowngrade()
        {
            var (result, _, _) = _migrator.Migrate(
                GameVersion.Major + 1,
                GameSaveData.Default(),
                SettingSaveData.Default());

            Assert.That(result, Is.EqualTo(SaveMigrator.MigrationResult.Downgrade));
        }

        [Test]
        public void Migrate_Downgrade_DataUnchanged()
        {
            var gameData = new GameSaveData(testScore: 50, testHighScore: 100);
            var settingsData = SettingSaveData.Default();

            var (_, returnedGame, _) = _migrator.Migrate(
                GameVersion.Major + 1,
                gameData,
                settingsData);

            Assert.That(returnedGame.TestScore, Is.EqualTo(50));
        }

        // -----------------------------------------------------------------------
        // Migration_1_to_2（現在のMajorが2以上の場合のみ有効）
        // -----------------------------------------------------------------------

        [Test]
        public void Migrate_FromVersion1_ReturnsSuccess()
        {
            // GameVersion.Major が 1 より大きい場合のみ意味がある
            if (GameVersion.Major <= 1)
                Assert.Ignore("GameVersion.Major is 1. Migration_1_to_2 not applicable.");

            var (result, _, _) = _migrator.Migrate(
                1,
                GameSaveData.Default(),
                SettingSaveData.Default());

            Assert.That(result, Is.EqualTo(SaveMigrator.MigrationResult.Success));
        }

        [Test]
        public void Migrate_FromVersion1_DataPreserved()
        {
            if (GameVersion.Major <= 1)
                Assert.Ignore("GameVersion.Major is 1. Migration_1_to_2 not applicable.");

            var gameData = new GameSaveData(testScore: 42, testHighScore: 99);
            var settingsData = new SettingSaveData(bgmVolume: 0.5f, seVolume: 0.5f);

            var (_, returnedGame, returnedSettings) = _migrator.Migrate(1, gameData, settingsData);

            // Migration_1_to_2 は変換なしでそのまま返すため値が保持される
            Assert.That(returnedGame.TestScore,     Is.EqualTo(42));
            Assert.That(returnedGame.TestHighScore, Is.EqualTo(99));
            Assert.That(returnedSettings.BGMVolume, Is.EqualTo(0.5f));
        }
    }
}