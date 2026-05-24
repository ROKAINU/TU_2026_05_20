using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using Game.Domain;
using Game.Application;
using Game.Application.Contracts;
using Game.Application.Runner;
using Game.Infrastructure;
using Game.Infrastructure.Save;
using Game.Infrastructure.AssetPreloader;
using Game.Presentation;
using Game.Presentation.SceneTransition;
using Game.Presentation.View;
using Game.Kernel;
using Game.Kernel.Utils.Abstract;
using Game.Kernel.Utils.R3;

namespace Game.Infrastructure.LifetimeScopes
{
    public class ProjectRootLifetimeScope : LifetimeScope
    {
        [SerializeField] private SceneCatalog sceneCatalog;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private BGMPlayer bgmPlayer;
        [SerializeField] private SEPlayer sePlayer;

        protected override void Configure(IContainerBuilder builder)
        {
            // マスターテーブル
            var ingredientMaster = CreateIngredientMasterTable();
            var drinkMaster = CreateDrinkMasterTable();

            builder.RegisterInstance(ingredientMaster);
            builder.RegisterInstance(drinkMaster);

            // ゲームロジック
            builder.Register<IIngredientInventory, IngredientInventory>(Lifetime.Singleton);
            builder.Register<DrinkCrafter>(Lifetime.Singleton);
            
            // MessagePipe 設定
            var options = builder.RegisterMessagePipe();

            builder.RegisterEvent<TransitionSceneMessage>(options);
            builder.RegisterEvent<TitleStartedMessage>(options);
            builder.RegisterEvent<ResultStartedMessage>(options);
            builder.RegisterEvent<GameStartedMessage>(options);
            builder.RegisterEvent<GamePauseMessage>(options);
            builder.RegisterEvent<GameFinishedMessage>(options);
        
            // ========================================
            // Domain層
            // ========================================
            
            builder.Register<Store<GameGlobalState>>(Lifetime.Singleton)
                .WithParameter(GameGlobalState.Default)
                .As<IStore<GameGlobalState>>().AsSelf();
            builder.Register<Store<GameSettingState>>(Lifetime.Singleton)
                .WithParameter(GameSettingState.Default)
                .As<IStore<GameSettingState>>().AsSelf();

            // ========================================
            // Kernel層：Logging
            // ========================================
            RegisterLogger(builder);

            // ========================================
            // Infrastructure層：永続化・UI
            // ========================================
            builder.RegisterInstance(sceneCatalog);
            builder.Register<SaveMigrator>(Lifetime.Singleton);
            builder.Register<ISaveService, PlayerPrefsSaveService>(Lifetime.Singleton);
            builder.Register<ISaveDataApplier, SaveDataApplier>(Lifetime.Singleton);
            builder.Register<SettingsDebouncedSaver>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SettingsDebouncedSaverEntryPoint>(Lifetime.Singleton);

            // ========================================
            // Infrastructure層：時間・乱数
            // ========================================
            builder.Register<IGameTime, UnityGameTime>(Lifetime.Singleton);
            builder.Register<IUnscaledTime, UnityUnscaledTime>(Lifetime.Singleton);
            builder.Register<IFixedTime, UnityFixedTime>(Lifetime.Singleton);
            builder.Register<IRandom, UnityRandomImpl>(Lifetime.Singleton);

            // ========================================
            // Presentation層
            // ========================================
            builder.RegisterInstance(audioMixer);
            builder.RegisterComponent(bgmPlayer);
            builder.RegisterComponent(sePlayer);
            builder.RegisterEntryPoint<AudioSettingsPresenter>()
                .As<IAudioSettingsPresenter>();
            builder.RegisterComponentInHierarchy<SceneTransitioner>();
            builder.RegisterComponentInHierarchy<ScreenFader>();

            // --- シーンロード戦略の切り替え ---
#if UNITY_ROOM
            // UnityRoom 向け：Addressables 非使用、Scenes in Build
            builder.Register<ISceneLoaderStrategy, BuildIndexSceneLoaderStrategy>(Lifetime.Singleton);
#else
            // 通常ビルド向け：Addressables + プリロード対応
            builder.Register<AddressablesAssetPreloader>(Lifetime.Singleton)
                .As<IAssetPreloader>();
            builder.Register<ISceneLoaderStrategy, AddressablesSceneLoaderStrategy>(Lifetime.Singleton);
#endif

        }

        /// <summary>
        /// ロガーを登録（ビルド設定に応じて実装を切り替え）。
        /// </summary>
        private static void RegisterLogger(IContainerBuilder builder)
        {
#if UNITY_EDITOR
            // 開発環境：すべてのログを表示
            builder.Register<LoggerBase, UnityDebugLogger>(Lifetime.Singleton);
#elif DEBUG
            // デバッグビルド：Warning 以上を表示
            builder.Register<LoggerBase>(Lifetime.Singleton, container =>
            {
                var logger = new UnityDebugLogger();
                logger.CurrentLogLevel = Game.Kernel.LogLevel.Warning;
                return logger;
            });
#else
            // リリースビルド：Error のみを表示
            builder.Register<LoggerBase>(Lifetime.Singleton, container =>
            {
                var logger = new UnityDebugLogger();
                logger.CurrentLogLevel = Game.Kernel.LogLevel.Error;
                return logger;
            });
#endif
        }

        // ===== Factory Methods =====
        private IngredientMasterTable CreateIngredientMasterTable()
        {
            var records = MasterDataLoader.Load<IngredientMasterRecord>(
                IngredientMasterRecord.FilePath);

            var dict = records
                .Select(IngredientMasterConverter.ToDomain)
                .ToDictionary(x => x.IngredientId);

            return new IngredientMasterTable(dict);
        }

        private DrinkMasterTable CreateDrinkMasterTable()
        {
            var records = MasterDataLoader.Load<DrinkMasterRecord>(
                DrinkMasterRecord.FilePath);

            var dict = records
                .Select(DrinkMasterConverter.ToDomain)
                .ToDictionary(x => x.DrinkId);

            return new DrinkMasterTable(dict);
        }
    }
}
