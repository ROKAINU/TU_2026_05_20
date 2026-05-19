using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using R3;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Presentation.View
{
    [RequireComponent(typeof(UIDocument))]
    internal class GameMainUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private Store<GameMainState> _gameMainStore;
        private Store<GameGlobalState> _globalStore;

        [Inject]
        public void Construct(Store<GameMainState> gameMainStore, Store<GameGlobalState> globalStore)
        {
            _gameMainStore = gameMainStore;
            _globalStore = globalStore;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            // UI Toolkitを「表示専用」にする：背面のuGUIへクリックを通す
            SetIgnorePickingRecursively(_uiDocument.rootVisualElement);
        }

        private static void SetIgnorePickingRecursively(VisualElement element)
        {
            element.pickingMode = PickingMode.Ignore;

            for (int i = 0; i < element.hierarchy.childCount; i++)
                SetIgnorePickingRecursively(element.hierarchy[i]);
        }

        private void Start()
        {
            var scoreText = _uiDocument.rootVisualElement.Q<Label>("score-text");
            var timerText = _uiDocument.rootVisualElement.Q<Label>("timer-text");

            _gameMainStore.State.Subscribe(state =>
            {
                var lestTime = state.RemainingTime;
                if (lestTime.Value < 0) lestTime = new RemainingTime(0);
                timerText.text = $"Time: {lestTime.Value:F2}";
            }).AddTo(destroyCancellationToken);

            _globalStore.State.Subscribe(state =>
            {
                scoreText.text = $"Score: {state.CurrentScore.Value}";
            }).AddTo(destroyCancellationToken);
        }
    }
}