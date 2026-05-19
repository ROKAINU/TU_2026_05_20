#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Kernel.Utils.Unity
{
    /// <summary>
    /// スプライトシートのスライスユーティリティ。
    /// グリッドレイアウト形式のスプライトシートから個別スプライトを生成。
    /// </summary>
    public static class SpriteSheetSlicer
    {
        /// <summary>
        /// スプライトシートをスライスして個別スプライトに分割。
        /// </summary>
        /// <param name="texture">スプライトシート テクスチャ</param>
        /// <param name="cellWidth">セルの幅（ピクセル）</param>
        /// <param name="cellHeight">セルの高さ（ピクセル）</param>
        /// <param name="pixelsPerUnit">ピクセルパーユニット（デフォルト 100）</param>
        /// <param name="pivot">ピボット位置（デフォルト 中央）</param>
        /// <returns>スライスされたスプライト配列（上行から順）</returns>
        /// <remarks>
        /// スプライトは左上から右下へ順に配置される。
        /// 例：4x2 シート（4列 2行）の場合、インデックスは以下の通り
        /// [0] [1] [2] [3]    ← 最上行
        /// [4] [5] [6] [7]    ← 下行
        /// 
        /// スプライト名は画面座標 [screenY_screenX] で表記
        /// （screenY: 画面上での行、screenX: 列）
        /// 上記の例では：
        /// [0_0] [0_1] [0_2] [0_3]    ← インデックス 0-3
        /// [1_0] [1_1] [1_2] [1_3]    ← インデックス 4-7
        /// 
        /// 注：テクスチャ内部ではY軸が逆（下から上へ）ですが、
        /// スプライト名は画面表示に合わせた座標を使用します。
        /// 
        /// ⚠️ 注意：この関数は初期化時に呼ぶことを推奨。
        /// 毎フレーム呼ぶと GC Allocation が発生します。
        /// </remarks>
        /// <exception cref="ArgumentNullException">texture が null の場合</exception>
        /// <exception cref="ArgumentException">
        /// パラメータが無効な場合。以下のいずれか：
        /// - cellWidth または cellHeight が 0 以下
        /// - pixelsPerUnit が 0 以下
        /// - テクスチャサイズがセルサイズで割り切れない
        /// - テクスチャが Read/Write 有効でない
        /// </exception>
        public static Sprite[] Slice(
            Texture2D texture,
            int cellWidth,
            int cellHeight,
            float pixelsPerUnit = 100f,
            Vector2? pivot = null)
        {
            // 入力パラメータの検証
            ValidateParameters(texture, cellWidth, cellHeight, pixelsPerUnit);

            var spritePivot = pivot ?? new Vector2(0.5f, 0.5f);

            int columns = texture.width / cellWidth;
            int rows = texture.height / cellHeight;

            var sprites = new List<Sprite>();

            // 上から順に（テクスチャの y=rows-1 から y=0 へ）
            for (int y = rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < columns; x++)
                {
                    var rect = new Rect(
                        x * cellWidth,
                        y * cellHeight,
                        cellWidth,
                        cellHeight
                    );

                    var sprite = Sprite.Create(
                        texture,
                        rect,
                        spritePivot,
                        pixelsPerUnit
                    );

                    sprite.name = $"{texture.name}_[{rows-1-y}_{x}]";
                    sprites.Add(sprite);
                }
            }

            return sprites.ToArray();
        }

        /// <summary>
        /// スプライトシートをスライスして2次元配列で返す。
        /// 行・列単位でアクセスしたい場合に便利。
        /// </summary>
        /// <remarks>
        /// sprites[y, x] でアクセス可能（y: 行、x: 列）
        /// 例：sprites[0, 0] = 最上行左端のセル
        /// 
        /// ⚠️ 注意：この関数は初期化時に呼ぶことを推奨。
        /// 毎フレーム呼ぶと GC Allocation が発生します。
        /// </remarks>
        public static Sprite[,] Slice2D(
            Texture2D texture,
            int cellWidth,
            int cellHeight,
            float pixelsPerUnit = 100f,
            Vector2? pivot = null)
        {
            // 入力パラメータの検証
            ValidateParameters(texture, cellWidth, cellHeight, pixelsPerUnit);

            var spritePivot = pivot ?? new Vector2(0.5f, 0.5f);

            int columns = texture.width / cellWidth;
            int rows = texture.height / cellHeight;

            var sprites = new Sprite[rows, columns];

            // 上から順に（y=0 が最上行）
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    // テクスチャ座標系（下から上へ）に変換
                    int texY = (rows - 1) - y;

                    var rect = new Rect(
                        x * cellWidth,
                        texY * cellHeight,
                        cellWidth,
                        cellHeight
                    );

                    var sprite = Sprite.Create(
                        texture,
                        rect,
                        spritePivot,
                        pixelsPerUnit
                    );

                    sprite.name = $"{texture.name}_[{y}_{x}]";
                    sprites[y, x] = sprite;
                }
            }

            return sprites;
        }

        /// <summary>
        /// パラメータの妥当性を検証。
        /// </summary>
        private static void ValidateParameters(
            Texture2D texture,
            int cellWidth,
            int cellHeight,
            float pixelsPerUnit)
        {
            // テクスチャの null チェック
            if (texture == null)
                throw new ArgumentNullException(nameof(texture), "Texture cannot be null");
            
            // テクスチャの Read/Write チェック
            if (!texture.isReadable)
                throw new ArgumentException(
                    $"Texture '{texture.name}' must have Read/Write Enabled in import settings"
                );
            
            // セルサイズの検証
            if (cellWidth <= 0 || cellHeight <= 0)
                throw new ArgumentException(
                    $"Cell width and height must be greater than 0. " +
                    $"Got width={cellWidth}, height={cellHeight}"
                );

            // PPUの検証
            if (pixelsPerUnit <= 0)
                throw new ArgumentException(
                    $"Pixels per unit must be greater than 0. Got {pixelsPerUnit}"
                );

            // テクスチャサイズの検証
            if (texture.width % cellWidth != 0 || texture.height % cellHeight != 0)
            {
                throw new ArgumentException(
                    $"Texture size ({texture.width}x{texture.height}) " +
                    $"must be perfectly divisible by cell size ({cellWidth}x{cellHeight})"
                );
            }
        }
    }
}