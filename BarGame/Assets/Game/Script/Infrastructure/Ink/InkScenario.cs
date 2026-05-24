using Ink.Runtime;
using System.Collections.Generic;
using System.Linq;
using Game.Application.Contracts;

namespace Game.Infrastructure.Ink
{ 
    /// <summary>
    /// Ink RuntimeをApplicationインターフェースにアダプト
    /// </summary>
    public sealed class InkScenario : IScenario
    {
        private readonly Story _story;
 
        public bool CanContinue => _story.canContinue;
        public bool HasChoices => _story.currentChoices != null && _story.currentChoices.Count > 0;
 
        public InkScenario(Story story)
        {
            _story = story;
        }
 
        public string GetNextLine()
        {
            if (!_story.canContinue)
                return null;
 
            string text = _story.Continue().Trim();
            while (string.IsNullOrWhiteSpace(text) && _story.canContinue)
                text = _story.Continue().Trim();
 
            return text;
        }
 
        public IReadOnlyList<string> GetChoices()
        {
            return _story.currentChoices
                .Cast<Choice>()
                .Select(c => c.text)
                .ToList();
        }
 
        public void SelectChoice(int index)
        {
            _story.ChooseChoiceIndex(index);
        }
 
        /// <summary>
        /// Ink外部関数をセットアップ
        /// </summary>
        public void BindExternalFunction(
            string functionName,
            System.Action action)
        {
            _story.BindExternalFunctionGeneral(
                functionName,
                _ =>
                {
                    action();
                    return null;
                });
        }

        public void BindExternalFunction<T>(
            string functionName,
            System.Action<T> action)
        {
            _story.BindExternalFunctionGeneral(
                functionName,
                args =>
                {
                    action((T)args[0]);
                    return null;
                });
        }

        public void BindExternalFunction<T1, T2>(
            string functionName,
            System.Action<T1, T2> action)
        {
            _story.BindExternalFunctionGeneral(
                functionName,
                args =>
                {
                    action(
                        (T1)args[0],
                        (T2)args[1]);

                    return null;
                });
        }
 
        public object GetVariable(string key)
        {
            return _story.variablesState[key];
        }
 
        public void SetVariable(string key, object value)
        {
            _story.variablesState[key] = value;
        }
    }
 
    /// <summary>
    /// Ink固有のセットアップロジック
    /// </summary>
    public sealed class InkScenarioFactory
    {
        public static InkScenario CreateFromJson(string inkJson)
        {
            var story = new Story(inkJson);
            return new InkScenario(story);
        }
    }
}