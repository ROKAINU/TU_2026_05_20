using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Game.Application.Contracts;

namespace Game.Presentation.UI
{
    /// <summary>
    /// Unity UIを使った表示実装
    /// </summary>
    public sealed class UnityStoryDisplay : MonoBehaviour, IStoryDisplay
    {
        [SerializeField] private TextMeshProUGUI _storyText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Transform _choiceContainer;
        [SerializeField] private Button _choiceButtonPrefab;
 
        private System.Action<int> _onChoiceSelected;
 
        public void DisplayText(string text)
        {
            _storyText.text = text;
        }
 
        public void DisplayName(string name)
        {
            _nameText.text = name;
        }
 
        public void DisplayBackground(string backgroundId)
        {
            // Presentation層でID→Spriteに解決
            var sprite = Resources.Load<Sprite>($"Backgrounds/{backgroundId}");
            if (sprite != null)
                _backgroundImage.sprite = sprite;
        }
 
        public void DisplayChoices(string[] choices, System.Action<int> onSelected)
        {
            _onChoiceSelected = onSelected;
 
            for (int i = 0; i < choices.Length; i++)
            {
                var button = Instantiate(_choiceButtonPrefab, _choiceContainer);
                int index = i;
 
                button.GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
                button.onClick.AddListener(() => onSelected(index));
            }
        }
 
        public void ClearChoices()
        {
            foreach (Transform child in _choiceContainer)
                Destroy(child.gameObject);
        }
    }
 
    /// <summary>
    /// Unity InputSystemを使った入力実装
    /// </summary>
    public sealed class UnityAdvanceInput : IAdvanceInput
    {
        public bool IsAdvancePressed()
        {
            // マウスクリック
            if (Mouse.current.leftButton.isPressed)
                return true;
 
            // またはキー入力
            if (Keyboard.current.spaceKey.isPressed)
                return true;
 
            return false;
        }
    }
}