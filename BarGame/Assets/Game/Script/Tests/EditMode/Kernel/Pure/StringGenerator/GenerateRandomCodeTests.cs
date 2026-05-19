#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Game.Kernel;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class GenerateRandomCodeTests
    {
        private const string ValidCodeChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string ValidIDChars = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";

        #region GenerateCode Tests

        /// <summary>
        /// GenerateCode：通常の長さでのコード生成
        /// </summary>
        [Test]
        public void GenerateCode_Length10_ReturnsString()
        {
            var code = GenerateRandomCode.GenerateCode(10);
            Assert.That(code, Is.Not.Null);
            Assert.That(code.Length, Is.EqualTo(10));
        }

        /// <summary>
        /// GenerateCode：長さ0での生成
        /// </summary>
        [Test]
        public void GenerateCode_Length0_ReturnsEmptyString()
        {
            var code = GenerateRandomCode.GenerateCode(0);
            Assert.That(code.Length, Is.EqualTo(0));
            Assert.That(code, Is.EqualTo(""));
        }

        /// <summary>
        /// GenerateCode：長さ1での生成
        /// </summary>
        [Test]
        public void GenerateCode_Length1_ReturnsSingleCharacter()
        {
            var code = GenerateRandomCode.GenerateCode(1);
            Assert.That(code.Length, Is.EqualTo(1));
            Assert.That(ValidCodeChars, Contains.Substring(code));
        }

        /// <summary>
        /// GenerateCode：生成される文字が正しいセットに含まれている
        /// </summary>
        [Test]
        public void GenerateCode_AllCharacters_AreInValidSet()
        {
            var code = GenerateRandomCode.GenerateCode(100);
            
            foreach (var ch in code)
            {
                Assert.That(ValidCodeChars, Contains.Substring(ch.ToString()),
                    $"Character '{ch}' is not in valid code character set");
            }
        }

        /// <summary>
        /// GenerateCode：指定Randomで決定的な生成
        /// </summary>
        [Test]
        public void GenerateCode_WithSameSeed_GeneratesSameCode()
        {
            var random1 = new Random(12345);
            var code1 = GenerateRandomCode.GenerateCode(20, random1);

            var random2 = new Random(12345);
            var code2 = GenerateRandomCode.GenerateCode(20, random2);

            Assert.That(code1, Is.EqualTo(code2));
        }

        /// <summary>
        /// GenerateCode：異なるSeedで異なるコードを生成（ほぼ常に）
        /// </summary>
        [Test]
        public void GenerateCode_WithDifferentSeeds_GeneratesDifferentCodes()
        {
            var random1 = new Random(111);
            var code1 = GenerateRandomCode.GenerateCode(20, random1);

            var random2 = new Random(222);
            var code2 = GenerateRandomCode.GenerateCode(20, random2);

            // ほぼ確実に異なる（完全には保証されないが、20文字なら十分な確率）
            Assert.That(code1, Is.Not.EqualTo(code2));
        }

        /// <summary>
        /// GenerateCode：複数回の生成がランダムに異なる
        /// </summary>
        [Test]
        public void GenerateCode_MultipleCalls_GeneratesDifferentCodes()
        {
            var codes = new HashSet<string>();
            
            for (int i = 0; i < 10; i++)
            {
                var code = GenerateRandomCode.GenerateCode(20);
                codes.Add(code);
            }

            // 10回の生成がすべて異なる（確率的にほぼ確実）
            Assert.That(codes.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// GenerateCode：長い文字列の生成
        /// </summary>
        [Test]
        public void GenerateCode_Length1000_GeneratesLongString()
        {
            var code = GenerateRandomCode.GenerateCode(1000);
            Assert.That(code.Length, Is.EqualTo(1000));
            
            foreach (var ch in code)
            {
                Assert.That(ValidCodeChars, Contains.Substring(ch.ToString()));
            }
        }

        /// <summary>
        /// GenerateCode：Randomなしでの生成（nullの場合）
        /// </summary>
        [Test]
        public void GenerateCode_WithoutRandom_GeneratesCode()
        {
            var code = GenerateRandomCode.GenerateCode(15, null);
            Assert.That(code.Length, Is.EqualTo(15));
            
            foreach (var ch in code)
            {
                Assert.That(ValidCodeChars, Contains.Substring(ch.ToString()));
            }
        }

        /// <summary>
        /// GenerateCode：パラメータなしでの生成
        /// </summary>
        [Test]
        public void GenerateCode_DefaultRandom_GeneratesCode()
        {
            var code = GenerateRandomCode.GenerateCode(15);
            Assert.That(code.Length, Is.EqualTo(15));
            
            foreach (var ch in code)
            {
                Assert.That(ValidCodeChars, Contains.Substring(ch.ToString()));
            }
        }

        #endregion

        #region GenerateID Tests

        /// <summary>
        /// GenerateID：通常の長さでのID生成
        /// </summary>
        [Test]
        public void GenerateID_Length10_ReturnsString()
        {
            var id = GenerateRandomCode.GenerateID(10);
            Assert.That(id, Is.Not.Null);
            Assert.That(id.Length, Is.EqualTo(10));
        }

        /// <summary>
        /// GenerateID：長さ0での生成
        /// </summary>
        [Test]
        public void GenerateID_Length0_ReturnsEmptyString()
        {
            var id = GenerateRandomCode.GenerateID(0);
            Assert.That(id.Length, Is.EqualTo(0));
            Assert.That(id, Is.EqualTo(""));
        }

        /// <summary>
        /// GenerateID：生成される文字が正しいセットに含まれている
        /// </summary>
        [Test]
        public void GenerateID_AllCharacters_AreInValidSet()
        {
            var id = GenerateRandomCode.GenerateID(100);
            
            foreach (var ch in id)
            {
                Assert.That(ValidIDChars, Contains.Substring(ch.ToString()),
                    $"Character '{ch}' is not in valid ID character set");
            }
        }

        /// <summary>
        /// GenerateID：紛らわしい文字 'I' が含まれていない
        /// </summary>
        [Test]
        public void GenerateID_DoesNotContain_LetterI()
        {
            // 紛らわしい文字を除外しているので、十分な長さで複数回テストして確認
            for (int i = 0; i < 10; i++)
            {
                var id = GenerateRandomCode.GenerateID(100);
                Assert.That(id, Does.Not.Contain("I"),
                    "Generated ID should not contain letter 'I'");
            }
        }

        /// <summary>
        /// GenerateID：紛らわしい文字 'O' が含まれていない
        /// </summary>
        [Test]
        public void GenerateID_DoesNotContain_LetterO()
        {
            for (int i = 0; i < 10; i++)
            {
                var id = GenerateRandomCode.GenerateID(100);
                Assert.That(id, Does.Not.Contain("O"),
                    "Generated ID should not contain letter 'O'");
            }
        }

        /// <summary>
        /// GenerateID：紛らわしい文字 'l' が含まれていない
        /// </summary>
        [Test]
        public void GenerateID_DoesNotContain_Letterl()
        {
            for (int i = 0; i < 10; i++)
            {
                var id = GenerateRandomCode.GenerateID(100);
                Assert.That(id, Does.Not.Contain("l"),
                    "Generated ID should not contain lowercase letter 'l'");
            }
        }

        /// <summary>
        /// GenerateID：紛らわしい文字 'o' が含まれていない
        /// </summary>
        [Test]
        public void GenerateID_DoesNotContain_Lettero()
        {
            for (int i = 0; i < 10; i++)
            {
                var id = GenerateRandomCode.GenerateID(100);
                Assert.That(id, Does.Not.Contain("o"),
                    "Generated ID should not contain lowercase letter 'o'");
            }
        }

        /// <summary>
        /// GenerateID：指定Randomで決定的な生成
        /// </summary>
        [Test]
        public void GenerateID_WithSameSeed_GeneratesSameID()
        {
            var random1 = new Random(12345);
            var id1 = GenerateRandomCode.GenerateID(20, random1);

            var random2 = new Random(12345);
            var id2 = GenerateRandomCode.GenerateID(20, random2);

            Assert.That(id1, Is.EqualTo(id2));
        }

        /// <summary>
        /// GenerateID：複数回の生成がランダムに異なる
        /// </summary>
        [Test]
        public void GenerateID_MultipleCalls_GeneratesDifferentIDs()
        {
            var ids = new HashSet<string>();
            
            for (int i = 0; i < 10; i++)
            {
                var id = GenerateRandomCode.GenerateID(20);
                ids.Add(id);
            }

            Assert.That(ids.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// GenerateID：長いID文字列の生成
        /// </summary>
        [Test]
        public void GenerateID_Length1000_GeneratesLongID()
        {
            var id = GenerateRandomCode.GenerateID(1000);
            Assert.That(id.Length, Is.EqualTo(1000));
            
            foreach (var ch in id)
            {
                Assert.That(ValidIDChars, Contains.Substring(ch.ToString()));
            }
        }

        /// <summary>
        /// GenerateID：Randomなしでの生成
        /// </summary>
        [Test]
        public void GenerateID_WithoutRandom_GeneratesID()
        {
            var id = GenerateRandomCode.GenerateID(15, null);
            Assert.That(id.Length, Is.EqualTo(15));
            
            foreach (var ch in id)
            {
                Assert.That(ValidIDChars, Contains.Substring(ch.ToString()));
            }
        }

        /// <summary>
        /// GenerateID：パラメータなしでの生成
        /// </summary>
        [Test]
        public void GenerateID_DefaultRandom_GeneratesID()
        {
            var id = GenerateRandomCode.GenerateID(15);
            Assert.That(id.Length, Is.EqualTo(15));
            
            foreach (var ch in id)
            {
                Assert.That(ValidIDChars, Contains.Substring(ch.ToString()));
            }
        }

        #endregion

        #region Comparison Tests

        /// <summary>
        /// GenerateCode と GenerateID の文字セット比較
        /// </summary>
        [Test]
        public void GenerateCode_Vs_GenerateID_CharacterSetDifference()
        {
            // GenerateID は I, O, l, o を除外している
            var codeCharSet = new HashSet<char>(ValidCodeChars);
            var idCharSet = new HashSet<char>(ValidIDChars);

            var excluded = codeCharSet.Except(idCharSet).ToList();
            
            Assert.That(excluded, Contains.Item('I'));
            Assert.That(excluded, Contains.Item('O'));
            Assert.That(excluded, Contains.Item('l'));
            Assert.That(excluded, Contains.Item('o'));
        }

        /// <summary>
        /// GenerateID は GenerateCode より文字セットが小さい
        /// </summary>
        [Test]
        public void GenerateID_HasSmallerCharacterSet_ThanGenerateCode()
        {
            Assert.That(ValidIDChars.Length, Is.LessThan(ValidCodeChars.Length));
            Assert.That(ValidCodeChars.Length - ValidIDChars.Length, Is.EqualTo(4)); // I, O, l, o
        }

        #endregion

        #region Edge Cases & Distribution

        /// <summary>
        /// 生成されたコードの文字分布（基本的なランダム性チェック）
        /// </summary>
        [Test]
        public void GenerateCode_Distribution_HasVariedCharacters()
        {
            var code = GenerateRandomCode.GenerateCode(1000);
            var uniqueChars = code.Distinct().Count();

            // 1000文字の中に、かなり多くの異なる文字が含まれるはず
            // (完全にランダムなら約42文字の期待値に近い)
            Assert.That(uniqueChars, Is.GreaterThan(20));
        }

        /// <summary>
        /// 生成されたIDの文字分布
        /// </summary>
        [Test]
        public void GenerateID_Distribution_HasVariedCharacters()
        {
            var id = GenerateRandomCode.GenerateID(1000);
            var uniqueChars = id.Distinct().Count();

            // IDも十分にランダムな分布を持つ
            Assert.That(uniqueChars, Is.GreaterThan(20));
        }

        /// <summary>
        /// 複数生成でコード衝突がないか（高い確率）
        /// </summary>
        [Test]
        public void GenerateCode_UniqueGeneration_LowCollisionRate()
        {
            var codes = new HashSet<string>();
            int iterations = 100;

            for (int i = 0; i < iterations; i++)
            {
                codes.Add(GenerateRandomCode.GenerateCode(32));
            }

            // 100回生成してほぼすべてが一意である
            Assert.That(codes.Count, Is.GreaterThan(95));
        }

        /// <summary>
        /// 複数生成でID衝突がないか（高い確率）
        /// </summary>
        [Test]
        public void GenerateID_UniqueGeneration_LowCollisionRate()
        {
            var ids = new HashSet<string>();
            int iterations = 100;

            for (int i = 0; i < iterations; i++)
            {
                ids.Add(GenerateRandomCode.GenerateID(32));
            }

            Assert.That(ids.Count, Is.GreaterThan(95));
        }

        /// <summary>
        /// 非常に長いコード生成
        /// </summary>
        [Test]
        public void GenerateCode_VeryLongLength_WorksCorrectly()
        {
            var code = GenerateRandomCode.GenerateCode(10000);
            Assert.That(code.Length, Is.EqualTo(10000));
        }

        /// <summary>
        /// 非常に長いID生成
        /// </summary>
        [Test]
        public void GenerateID_VeryLongLength_WorksCorrectly()
        {
            var id = GenerateRandomCode.GenerateID(10000);
            Assert.That(id.Length, Is.EqualTo(10000));
        }

        #endregion

        #region Determinism Tests

        /// <summary>
        /// 同じRandomインスタンスで連続生成（状態が進む）
        /// </summary>
        [Test]
        public void GenerateCode_SequentialGeneration_WithSameRandom_DifferentResults()
        {
            var random = new Random(999);
            
            var code1 = GenerateRandomCode.GenerateCode(20, random);
            var code2 = GenerateRandomCode.GenerateCode(20, random);
            var code3 = GenerateRandomCode.GenerateCode(20, random);

            // Randomの状態が進むので、異なる結果が得られる
            Assert.That(code1, Is.Not.EqualTo(code2));
            Assert.That(code2, Is.Not.EqualTo(code3));
            Assert.That(code1, Is.Not.EqualTo(code3));
        }

        /// <summary>
        /// 異なるRandomインスタンスで同じSeedなら同じ結果
        /// </summary>
        [Test]
        public void GenerateID_SeparateRandomInstances_SameSeed_SameResult()
        {
            var code1 = GenerateRandomCode.GenerateID(50, new Random(777));
            var code2 = GenerateRandomCode.GenerateID(50, new Random(777));
            var code3 = GenerateRandomCode.GenerateID(50, new Random(777));

            Assert.That(code1, Is.EqualTo(code2));
            Assert.That(code2, Is.EqualTo(code3));
        }

        #endregion
    }
}