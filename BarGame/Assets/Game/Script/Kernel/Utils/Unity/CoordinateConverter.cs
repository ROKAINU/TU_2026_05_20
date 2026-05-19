#nullable enable
using UnityEngine;

namespace Game.Kernel.Utils.Unity
{
    /// <summary>
    /// Kernel座標と Unity座標の相互変換ユーティリティ。
    /// </summary>

    public static class CoordinateConverter
    {
        /// <summary>
        /// Domain の Int2 を Unity の Vector2Int に変換。
        /// </summary>
        public static Vector2Int ToUnity(Int2 coord) => new(coord.x, coord.y);

        /// <summary>
        /// Unity の Vector2Int を Domain の Int2 に変換。
        /// </summary>
        public static Int2 FromUnity(Vector2Int v) => new(v.x, v.y);

        /// <summary>
        /// Domain の Coord2D を Unity の Vector2 に変換。
        /// </summary>
        public static Vector2 ToUnity(Coord2D coord) => new(coord.x, coord.y);

        /// <summary>
        /// Unity の Vector2 を Domain の Coord2D に変換。
        /// </summary>
        public static Coord2D FromUnity(Vector2 v) => new(v.x, v.y);
    }
}