using System;
using System.Collections.Generic;

namespace Game.Application.Runner
{
    public class GameState
    {
        public enum State
        {
            Playing,
            Paused
            // ...必要に応じて拡張
        }

        // 遷移可能な次のステートを定義
        private readonly Dictionary<State, State[]> transitions = new()
        {
            { State.Playing, new[] { State.Paused } },      // Playing→Paused
            { State.Paused, new[] { State.Playing } }       // Paused→Playing
        };

        // 現在のステート
        private State currentState;
        public State CurrentState => currentState;

        /// <summary>新しいステートに入った時に発火</summary>
        public event Action<State> OnStateEntered;

        /// <summary>ステートが変更された時に発火 (from, to)</summary>
        public event Action<State, State> OnStateChanged;

        /// <summary>古いステートから抜ける時に発火</summary>
        public event Action<State> OnStateExited;

        // ログ（デバッグ・テスト用）
        public List<(State from, State to)> TransitionHistory { get; } = new();

        public GameState(State state)
        {
            currentState = state;
        }

        /// <summary>
        /// ステートを変更する。遷移が許可されていない場合は変更せずにfalseを返す。
        /// </summary>
        /// <param name="newState">変更後のステート</param>
        /// <returns>ステートが正常に変更された場合はtrue、それ以外の場合はfalse</returns>
        public bool ChangeState(State newState)
        {
            if (!CanTransition(newState))
                return false;

            State previousState = currentState;
            
            OnStateChanged?.Invoke(currentState, newState);
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(currentState);

            // ログ記録
            TransitionHistory.Add((previousState, newState));

            return true;
        }

        /// <summary>
        /// 指定されたステートへの遷移が可能かどうかを判定する。
        /// </summary>
        /// <param name="targetState">移動先のステート</param>
        /// <returns>遷移が可能場合はtrue、それ以外の場合はfalse</returns>
        public bool CanTransition(State targetState)
        {
            return transitions.ContainsKey(currentState) &&
                Array.Exists(transitions[currentState], s => s == targetState);
        }
        
        /// <summary>
        /// ステートに入るときの処理。ここでイベントを発火させる。必要に応じて、ステートごとの処理を追加しても良い。
        /// </summary>
        /// <param name="state">Entering state</param>
        private void OnEnterState(State state)
        {
            // ステート開始時の処理
            OnStateEntered?.Invoke(state);
        }

        /// <summary>
        /// ステートから抜けるときの処理。ここでイベントを発火させる。必要に応じて、ステートごとの処理を追加しても良い。
        /// </summary>
        /// <param name="state">Exiting state</param>
        private void OnExitState(State state)
        {
            // ステート終了時の処理
            OnStateExited?.Invoke(state);
        }
    }
}