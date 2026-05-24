// Tests/EditMode/InventoryTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using Game.Domain;
using Game.Infrastructure;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// IngredientInventory の在庫管理テスト
    /// 
    /// テスト内容
    /// Add_在庫を追加できるか
    ///     Add(Whiskey, 3) 後に GetStock が 3 を返すか
    /// Add_同じ素材を複数回追加すると累積されるか
    ///     2回に分けて追加した合計が正しいか
    /// HasIngredient_在庫があればtrueを返すか
    ///     1個以上あれば true を返すか
    /// HasIngredient_在庫がなければfalse
    ///     追加していない素材が false を返すか
    /// ConsumeMultiple_在庫が足りれば消費してtrueを返す
    ///     消費後に在庫が1減っているか、戻り値が true か
    /// ConsumeMultiple_在庫が足りなければfalseを返し消費しない
    ///     1つでも足りない素材があれば他の素材も消費しないか
    /// GetStock_登録していない素材は0を返す
    ///     未登録の素材に 0 が返るか（例外が出ないか）
    /// </summary>
    public class InventoryTests
    {
        private IngredientInventory _inventory;

        [SetUp]
        public void SetUp()
        {
            _inventory = new IngredientInventory();
        }

        [Test]
        public void Add_在庫を追加できる()
        {
            _inventory.Add(IngredientId.Whiskey, 3);
            Assert.AreEqual(3, _inventory.GetStock(IngredientId.Whiskey));
        }

        [Test]
        public void Add_同じ素材を複数回追加すると累積される()
        {
            _inventory.Add(IngredientId.Whiskey, 2);
            _inventory.Add(IngredientId.Whiskey, 3);
            Assert.AreEqual(5, _inventory.GetStock(IngredientId.Whiskey));
        }

        [Test]
        public void HasIngredient_在庫があればtrue()
        {
            _inventory.Add(IngredientId.Gin, 1);
            Assert.IsTrue(_inventory.HasIngredient(IngredientId.Gin));
        }

        [Test]
        public void HasIngredient_在庫がなければfalse()
        {
            Assert.IsFalse(_inventory.HasIngredient(IngredientId.Gin));
        }

        [Test]
        public void ConsumeMultiple_在庫が足りれば消費してtrueを返す()
        {
            _inventory.Add(IngredientId.Whiskey, 2);
            _inventory.Add(IngredientId.Soda, 2);

            var result = _inventory.ConsumeMultiple(new[]
            {
                IngredientId.Whiskey,
                IngredientId.Soda
            });

            Assert.IsTrue(result);
            Assert.AreEqual(1, _inventory.GetStock(IngredientId.Whiskey));
            Assert.AreEqual(1, _inventory.GetStock(IngredientId.Soda));
        }

        [Test]
        public void ConsumeMultiple_在庫が足りなければfalseを返し消費しない()
        {
            _inventory.Add(IngredientId.Whiskey, 1);
            // Sodaは在庫なし

            var result = _inventory.ConsumeMultiple(new[]
            {
                IngredientId.Whiskey,
                IngredientId.Soda
            });

            Assert.IsFalse(result);
            // Whiskey は消費されていないこと
            Assert.AreEqual(1, _inventory.GetStock(IngredientId.Whiskey));
        }

        [Test]
        public void GetStock_登録していない素材は0を返す()
        {
            Assert.AreEqual(0, _inventory.GetStock(IngredientId.Campari));
        }
    }
}