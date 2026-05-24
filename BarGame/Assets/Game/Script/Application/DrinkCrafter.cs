using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain;
using Game.Application.Contracts;

namespace Game.Application
{
    public sealed class DrinkCrafter
    {
        private readonly DrinkMasterTable _drinkMaster;
        private readonly IIngredientInventory _inventory;

        public DrinkCrafter(DrinkMasterTable drinkMaster, IIngredientInventory inventory)
        {
            _drinkMaster = drinkMaster;
            _inventory = inventory;
        }

        // レシピから作れるか検証
        public bool CanCraft(DrinkId id)
        {
            if (!_drinkMaster.TryGet(id, out var drink))
                return false;

            // 全ての素材が揃っているか
            return drink.Ingredients.All(ing => _inventory.HasIngredient(ing));
        }

        // 作れるドリンク一覧
        public IEnumerable<DrinkId> GetAvailableDrinks()
        {
            return _drinkMaster.All.Keys.Where(CanCraft);
        }

        // ドリンクを調合（素材を消費）
        public bool TryCraft(DrinkId id, out DrinkData craftedDrink)
        {
            craftedDrink = null;

            if (!_drinkMaster.TryGet(id, out var drink))
                return false;

            // 素材をまとめて消費
            if (!_inventory.ConsumeMultiple(drink.Ingredients))
                return false;

            // 成功
            craftedDrink = drink;
            return true;
        }
    }
}