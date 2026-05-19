#nullable enable
using System;
using System.Collections.Generic;

namespace Game.Kernel
{
    /// <summary>
    /// セル判定述語。
    /// 指定座標が条件を満たすかを判定。
    /// </summary>
    /// <param name="coordinate">判定する座標</param>
    /// <returns>その座標が障害物である場合は true</returns>
    public delegate bool CellBlockedPredicate(Int2 coordinate);

    /// <summary>
    /// パス検証ユーティリティ。
    /// グリッド内のパスが条件を満たしているか検証する。
    /// Unity非依存の汎用アルゴリズム。
    /// </summary>
    public static class PathValidator
    {
        /// <summary>
        /// パスがすべての条件を満たしているか検証。
        /// </summary>
        /// <param name="path">検証対象のパス</param>
        /// <param name="cellFilter">セル判定関数（true=ブロック）</param>
        /// <param name="allowDiagonal">斜め移動を許可するか</param>
        /// <param name="validateStartCell">開始セルを検証するか</param>
        /// <param name="validateEndCell">終了セルを検証するか</param>
        /// <exception cref="ArgumentNullException">cellFilter が null の場合</exception>
        /// <returns>検証結果</returns>
        public static Result<Unit> Validate(
            List<Int2> path,
            CellBlockedPredicate cellFilter,
            bool allowDiagonal = true,
            bool validateStartCell = false,
            bool validateEndCell = false)
        {    
            // Null/Empty チェック
            if (path == null || path.Count == 0)
                return Result<Unit>.Err("Path is null or empty.");
            
            // セルフィルタの null チェック
            if (cellFilter == null)
                throw new ArgumentNullException(nameof(cellFilter));

            // 単一セルの場合
            if (path.Count == 1)
            {
                if (!validateStartCell && !validateEndCell)
                    return Result<Unit>.Ok(Unit.Value);

                if (cellFilter(path[0]))
                    return Result<Unit>.Err($"Cell blocked at {path[0]}");
                
                return Result<Unit>.Ok(Unit.Value);
            }

            // 開始セル検証：validateStartCell フラグで制御
            if (validateStartCell && cellFilter(path[0]))
                return Result<Unit>.Err($"Start cell blocked at {path[0]}");
            
            // 各セグメントを検証
            for (int i = 0; i < path.Count - 1; i++)
            {
                Int2 current = path[i];
                Int2 next = path[i + 1];

                // セルの隣接チェック
                if (Math.Abs(current.x - next.x) > 1 || Math.Abs(current.y - next.y) > 1)
                    return Result<Unit>.Err($"Non-adjacent cells between {current} and {next}");

                // 重複セルのチェック
                if (current.Equals(next))
                    return Result<Unit>.Err($"Duplicate cell at {current}");

                // ループ内で中間セルのチェック：i > 0 で開始セルをスキップ
                if (i > 0 && cellFilter(current))
                    return Result<Unit>.Err($"Cell blocked at {current}");

                // 斜め移動の隅をチェック
                if (allowDiagonal && IsMovingDiagonally(current, next) && IsBlockedByCorner(current, next, cellFilter))
                    return Result<Unit>.Err($"Corner blocked between {current} and {next}");
            }

            // 終了セル検証
            if (validateEndCell && cellFilter(path[^1]))
                return Result<Unit>.Err($"End cell blocked at {path[^1]}");

            return Result<Unit>.Ok(Unit.Value);
        }

        private static bool IsMovingDiagonally(Int2 from, Int2 to)
            => Math.Abs(from.x - to.x) == 1 && Math.Abs(from.y - to.y) == 1;

        private static bool IsBlockedByCorner(Int2 from, Int2 to, CellBlockedPredicate cellFilter)
        {
            var corner1 = new Int2(from.x, to.y);
            var corner2 = new Int2(to.x, from.y);
            return cellFilter(corner1) && cellFilter(corner2);
        }
    }

    /// <summary>
    /// 単位型（値を持たない）。
    /// </summary>
    public struct Unit
    {
        public static readonly Unit Value = default;
    }
}