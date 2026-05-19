#nullable enable
using UnityEngine.InputSystem;
using Game.Domain;
using Game.Kernel;

namespace Game.Infrastructure
{
    /// <summary>
    /// リアル入力を提供するサービス。
    /// マウスクリックなどの実際のプレイヤー入力を処理。
    /// </summary>
    public sealed class RealInputService : IInputService
    {
        public bool GetAddScoreInput()
        {
            // マウス左クリック
            return  (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) || 
                    (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                    (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
        }

        public Coord2D GetMovementInput()
        {
            // WASD キーでの移動入力
            float x = 0f;
            float y = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) y += 1f;
                if (Keyboard.current.sKey.isPressed) y -= 1f;
                if (Keyboard.current.aKey.isPressed) x -= 1f;
                if (Keyboard.current.dKey.isPressed) x += 1f;
            }

            if (x == 0 && y == 0)
                return Coord2D.Zero;

            return new Coord2D(x, y).Normalize();
        }

        public bool IsPausePressed()
        {
            // ESC キー
            return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        }
    }
}