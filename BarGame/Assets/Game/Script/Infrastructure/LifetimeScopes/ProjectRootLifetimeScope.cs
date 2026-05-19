using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using Game.Domain;
using Game.Application;
using Game.Application.Contracts;
using Game.Infrastructure.Save;
using Game.Infrastructure.AssetPreloader;
using Game.Presentation;
using Game.Presentation.SceneTransition;
using Game.Presentation.View;
using Game.Kernel;
using Game.Kernel.Utils.Abstruct;
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
            // MessagePipe 設定
            var messagePipeOptions = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<TransitionSceneMessage>(messagePipeOptions);

            // ========================================
            // Kernel層：Logging
            // ========================================
            RegisterLogger(builder);

            // ========================================
            // Infrastructure層：永続化・UI
            // ========================================
            builder.RegisterInstance(sceneCatalog);
            builder.RegisterComponentInHierarchy<SceneTransitioner>();
            builder.RegisterComponentInHierarchy<ScreenFader>();
            builder.Register<Store<GameGlobalState>>(Lifetime.Singleton)
                .WithParameter(GameGlobalState.Default);
            builder.Register<Store<GameSettingState>>(Lifetime.Singleton)
                .WithParameter(GameSettingState.Default);
            builder.Register<SaveMigrator>(Lifetime.Singleton);
            builder.Register<ISaveService, PlayerPrefsSaveService>(Lifetime.Singleton);
            builder.Register<ISaveDataApplier, SaveDataApplier>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AudioSettingsPresenter>()
                .As<IAudioSettingsPresenter>();
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
            // Presentation層：Audio
            // ========================================
            builder.RegisterInstance(audioMixer);
            builder.RegisterComponent(bgmPlayer);
            builder.RegisterComponent(sePlayer);

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
    }
}
