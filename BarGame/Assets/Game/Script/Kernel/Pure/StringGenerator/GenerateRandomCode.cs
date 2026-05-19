#nullable enable
using System;

namespace Game.Kernel
{
    public static class GenerateRandomCode
    {
        private static readonly string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly string ids = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";//IOloは0,1と紛らわしいので除外
        
        /// <summary>
        /// 指定された文字列セットからランダムなコードを生成します。
        /// </summary>
        /// <param name="length">生成する文字列の長さ。</param>
        /// <param name="random">乱数生成器（省略時は新規インスタンスを生成）。</param>
        /// <returns>ランダム文字列。</returns>
        public static string GenerateCode(int length, Random? random = null)
        {
            return Generate(length, chars, random ?? new Random());
        }

        /// <summary>
        /// バイアスのない文字列 (紛らわしい文字を除去) のランダム生成に対応。
        /// </summary>
        /// <param name="length">生成する文字列の長さ。</param>
        /// <param name="random">乱数生成器（省略時は新規インスタンスを生成）。</param>
        /// <returns>ランダム文字列。</returns>
        public static string GenerateID(int length, Random? random = null)
        {
            return Generate(length, ids, random ?? new Random());
        }

        /// <summary>
        /// 内部ユーティリティ: 指定した文字セットを用いてランダム文字列を生成。
        /// </summary>
        private static string Generate(int length, string charset, Random random)
        {
            var code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = charset[random.Next(charset.Length)];
            }
            return new string(code);
        }
    }
}