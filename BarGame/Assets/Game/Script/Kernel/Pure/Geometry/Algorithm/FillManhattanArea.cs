using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Kernel
{
    /// <summary>
    /// マンハッタン距離で指定された範囲を塗りつぶすアルゴリズム。
    /// 中心からの距離が r 以下のセルを対象とする（菱形の範囲）。
    /// グリッドの境界を超えるセルは無視される。
    /// </summary>
    public static class FillManhattanArea
    {
        public static void Fill<T>(T[,] grid, int cx, int cy, int r, T value)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            if (cx < 0 || cx >= width) throw new ArgumentOutOfRangeException(nameof(cx), "cx is out of grid bounds.");
            if (cy < 0 || cy >= height) throw new ArgumentOutOfRangeException(nameof(cy), "cy is out of grid bounds.");

            for (int dx = -r; dx <= r; dx++)
            {
                int remaining = r - Math.Abs(dx);

                int x = cx + dx;
                if (!IsInBounds(x, cy, width, height)) continue;

                for (int dy = -remaining; dy <= remaining; dy++)
                {
                    int y = cy + dy;
                    if (!IsInBounds(x, y, width, height)) continue;

                    grid[x, y] = value;
                }
            }
        }

        public static List<Int2> FillPosition<T>(T[,] grid, int cx, int cy, int r)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            
            if (cx < 0 || cx >= width) throw new ArgumentOutOfRangeException(nameof(cx), "cx is out of grid bounds.");
            if (cy < 0 || cy >= height) throw new ArgumentOutOfRangeException(nameof(cy), "cy is out of grid bounds.");

            List<Int2> positions = new List<Int2>();

            for (int dx = -r; dx <= r; dx++)
            {
                int remaining = r - Math.Abs(dx);

                int x = cx + dx;
                if (!IsInBounds(x, cy, width, height)) continue;

                for (int dy = -remaining; dy <= remaining; dy++)
                {
                    int y = cy + dy;
                    if (!IsInBounds(x, y, width, height)) continue;

                    positions.Add(new Int2(x, y));
                }
            }
            return positions;
        }

        private static bool IsInBounds(int x, int y, int width, int height)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}