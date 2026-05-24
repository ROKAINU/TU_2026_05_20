using System;
using System.Collections.Generic;

namespace Game.Infrastructure
{
    /// <summary>
    /// テスト用の入力シーケンス定義
    /// </summary>
    public interface IInputSequence
    {
        /// <summary>シーケンスの名前</summary>
        string Name { get; }
        
        /// <summary>実行する入力アクションのリスト</summary>
        IReadOnlyList<InputAction> Actions { get; }
        
        /// <summary>シーケンス全体の実行が完了したかどうか</summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 次のアクションを取得
        /// </summary>
        /// <param name="deltaTime">フレーム時間</param>
        /// <returns>(アクション, 実行可能か)</returns>
        (InputAction? action, bool isReady) GetNextAction(float deltaTime);
        
        /// <summary>シーケンスをリセット</summary>
        void Reset();
    }

    /// <summary>
    /// 基本的なInputSequence実装
    /// </summary>
    public class InputSequence : IInputSequence
    {
        private int _currentActionIndex;
        private float _currentDelayTimer;
        private int _currentRepeatCount;
        public string Name { get; }
        public IReadOnlyList<InputAction> Actions { get; }
        public bool IsCompleted => _currentActionIndex >= Actions.Count;
        
        public InputSequence(string name, params InputAction[] actions)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name must not be null or empty.", nameof(name));
            
            Name = name;
            Actions = actions ?? Array.Empty<InputAction>();
            Reset();
        }

        public void Reset()
        {
            _currentActionIndex = 0;
            _currentDelayTimer = 0f;
            _currentRepeatCount = 0;
        }

        /// <summary>
        /// 次の実行待機中のアクションを取得
        /// デルタ時間を加算して待機時間を計算
        /// タイマーはリセットされ、超過分は失われます
        /// </summary>
        public (InputAction? action, bool isReady) GetNextAction(float deltaTime)
        {
            if (IsCompleted)
                return (null, false);
            
            if (deltaTime < 0)
                throw new ArgumentException("DeltaTime must be non-negative.", nameof(deltaTime));
            
            _currentDelayTimer += deltaTime;
            var currentAction = Actions[_currentActionIndex];
            
            if (_currentDelayTimer >= currentAction.DelaySeconds)
            {
                _currentRepeatCount++;
                
                if (_currentRepeatCount >= currentAction.RepeatCount)
                {
                    // 次のアクションへ進み、タイマーはリセット
                    _currentActionIndex++;
                    _currentDelayTimer = 0f;
                    _currentRepeatCount = 0;
                }
                else
                {
                    // 同じアクションをもう一度実行、タイマーは次の遅延のためリセット
                    _currentDelayTimer = 0f;
                }
                
                return (currentAction, true);
            }
            
            return (currentAction, false);
        }
    }
}