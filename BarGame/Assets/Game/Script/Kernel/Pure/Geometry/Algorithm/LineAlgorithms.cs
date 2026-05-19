using System;
using System.Collections.Generic;

namespace Game.Kernel
{
    /// <summary>
    /// Bresenham アルゴリズムを使用した直線アルゴリズム。
    /// </summary>
    public static class LineAlgorithms
    {
        /// <summary>
        /// Bresenham のアルゴリズムで2点間の直線上のセルを取得。
        /// </summary>
        /// <param name="from">開始位置</param>
        /// <param name="to">終了位置</param>
        /// <returns>直線上のセル座標リスト</returns>
        /// https://ja.wikipedia.org/wiki/%E3%83%96%E3%83%AC%E3%82%BC%E3%83%B3%E3%83%8F%E3%83%A0%E3%81%AE%E3%82%A2%E3%83%AB%E3%82%B4%E3%83%AA%E3%82%BA%E3%83%A0
        /// これを参考にした
        public static List<Int2> GetLine(Int2 from, Int2 to)
        {
            var result = new List<Int2>();
            
            int dx = Math.Abs(from.x - to.x);
            int dy = Math.Abs(from.y - to.y);
            int sx = (from.x < to.x) ? 1 : -1;
            int sy = (from.y < to.y) ? 1 : -1;
            int err = dx - dy;

            int x0 = from.x;
            int y0 = from.y;
            
            while(true)
            {
                result.Add(new Int2(x0, y0));
                if ((x0 == to.x) && (y0 == to.y)) break;

                var e2 = 2*err;
                if(e2 > -dy)
                {
                    err = err - dy;
                    x0 += sx;
                }
                if(e2 < dx)
                {
                    err = err + dx;
                    y0 += sy;
                }
            }

            return result;
        }
    }   
}