using System;
using Game;
using Game.Domain;
using MemoryPack;

namespace Game.Infrastructure.Save
{
    public static class SaveHub
    {
        public static SaveGameVersion CurrentVersionToSave()
        {
            return new SaveGameVersion
            {
                Major = GameVersion.Major,
                Minor = GameVersion.Minor,
                Patch = GameVersion.Patch
            };
        }

        public static SaveGameSaveData ToSave(this GameSaveData src)
        {
            return new SaveGameSaveData
            {
                TestScore = src.TestScore,
                TestHighScore = src.TestHighScore
            };
        }

        public static GameSaveData ToGame(this SaveGameSaveData src)
        {
            return new GameSaveData(src.TestScore, src.TestHighScore);
        }
        
        public static SaveSettingSaveData ToSave(this SettingSaveData src)
        {
            return new SaveSettingSaveData
            {
                BGMVolume = src.BGMVolume,
                SEVolume = src.SEVolume
            };
        }
        
        public static SettingSaveData ToGame(this SaveSettingSaveData src)
        {
            return new SettingSaveData(src.BGMVolume, src.SEVolume);
        }
    }

    [MemoryPackable]
    public partial class SaveFileRoot
    {
        public SaveGameVersion Version = SaveHub.CurrentVersionToSave();
        public SaveGameSaveData GameData = GameSaveData.Default().ToSave();
        public SaveSettingSaveData SettingData = SettingSaveData.Default().ToSave();
    }

    [MemoryPackable]
    public partial class SaveGameVersion
    {
        public int Major;
        public int Minor;
        public int Patch;
    }

    [MemoryPackable]
    public partial class SaveGameSaveData
    {
        public int TestScore;
        public int TestHighScore;
    }

    [MemoryPackable]
    public partial class SaveSettingSaveData
    {
        public float BGMVolume;
        public float SEVolume;
    }
}