using Enums;
using Gameplay;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using Service;
using System;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(InformationWindow))]
    public class InfoWindowHandler : MonoBehaviour
    {
        private InformationWindow _informationWindow;

        public static event Func<ItemType, bool> IsGenerator;
        public static event Func<ItemType, int, bool> IsGeneratorMaxLevel;
        public static event Func<ItemType, int> GetGeneratorLevel;
        public static event Func<ItemType, Sprite[]> GetProducedItemSprites;

        private void Awake()
        {
            _informationWindow = GetComponent<InformationWindow>();
        }

        private void OnEnable()
        {
            Item.CursorHoveredMovableItem += OnCursorHoveredItem;
            Item.CursorHoveredNotMovableItem += OnCursorHoveredItem;
            Item.CursorLeftItem += _informationWindow.Hide;
            Upgradable.CursorHoveredGenerator += OnCursorHoveredGenerator;
            Upgradable.CursorLeftGenerator += _informationWindow.Hide;
        }

        private void OnDisable()
        {
            Item.CursorHoveredMovableItem -= OnCursorHoveredItem;
            Item.CursorHoveredNotMovableItem -= OnCursorHoveredItem;
            Item.CursorLeftItem -= _informationWindow.Hide;
            Upgradable.CursorHoveredGenerator -= OnCursorHoveredGenerator;
            Upgradable.CursorLeftGenerator -= _informationWindow.Hide;
        }

        private void OnCursorHoveredItem(ItemType type, int level)
        {
            var storage = GameStorage.Instanse;
            var itemDescription = Translation.GetItemDescription(type, level);
            var instruction = "";
            var maxLevel = storage.IsItemMaxLevel(type, level);
            Translation.ItemDescription nextDescription = null;

            if (!maxLevel)
                nextDescription = Translation.GetItemDescription(type, level + 1);

            var isGenerator = IsGenerator?.Invoke(type);
            if (isGenerator.GetValueOrDefault())
            {
                var isGeneratorMaxLevel = IsGeneratorMaxLevel?.Invoke(type, level);
                if (isGeneratorMaxLevel.GetValueOrDefault())
                    instruction = $"Перетащи на генератор «{itemDescription.Title}», чтобы улучшить его.\n" +
                        $"Этот предмет нельзя выбросить.";
                else
                {
                    var currentLevel = GetGeneratorLevel?.Invoke(type);
                    var titleNextLevel = Translation.GetItemDescription(type, level + 1).Title;
                    var titleNeedLevel = Translation.GetItemDescription(type, currentLevel.GetValueOrDefault()).Title;
                    instruction = $"Объедини, чтобы получить предмет «{titleNextLevel}» {level + 1}-го уровня.\n" +
                        $"Получи «{titleNeedLevel}» {currentLevel.GetValueOrDefault()}-го уровня, чтобы улучшить генератор «{titleNeedLevel}».\n" +
                        $"Этот предмет нельзя выбросить.";
                }
            }
            else if (type == ItemType.Star || type == ItemType.Brilliant)
            {
                var currencyCount = type == ItemType.Star ? storage.GetStarsRewardByItemLevel(level) : storage.GetBrilliantsRewardByItemlevel(level);
                if (maxLevel)
                    instruction = $"Нажми, чтобы получить {Translation.GetItemTitle(type)} ({currencyCount}).";
                else
                    instruction = $"Нажми, чтобы получить {Translation.GetItemTitle(type)} " +
                        $"({currencyCount}), или объедини, чтобы их стало больше.";
            }
            else if (type == ItemType.Present)
            {
                if (maxLevel)
                    instruction = "Нажми, чтобы открыть.";
                else
                    instruction = "Нажми, чтобы открыть, или объедини, чтобы получить более ценный подарок.";
            }
            else if (type == ItemType.OpenPresent)
                instruction = "Нажми, чтобы получить награды!";
            else if (type == ItemType.Key)
            {
                if (maxLevel)
                    instruction = $"Перетащи на замок {level}-го уровня, чтобы разблокировать ячейку.";
                else
                    instruction = $"Перетащи на замок {level}-го уровня, чтобы разблокировать ячейку, " +
                        $"или объедини, чтобы открывать замки более высоких уровней.";
            }
            else if (type == ItemType.Lock)
                instruction = $"Перетащи сюда ключ {level}-го уровня, чтобы разблокировать ячейку.";
            else if (type == ItemType.Box)
            {
                if (maxLevel)
                    instruction = "Нажми, чтобы получить предмет максимального уровня, необходимый для выполнения заказа.";
                else if (level == 2)
                    instruction = $"Нажми, чтобы получить случайный предмет, необходимый для выполнения заказа, или объедини, чтобы получить «{nextDescription.Title}».";
                else if (level == 1)
                    instruction = $"Объедини, чтобы получить «{nextDescription.Title}».";
            }
            else
            {
                if (maxLevel)
                    instruction = $"Перетащи на окно заказа, чтобы выполнить его, если он содержит «{itemDescription.Title}».";
                else
                    instruction = $"Перетащи на окно заказа, чтобы выполнить его, если он содержит «{itemDescription.Title}», или объедини, чтобы получить «{nextDescription.Title}».";
            }

            _informationWindow.ShowItem(itemDescription.Title, level, itemDescription.Description, instruction);
        }

        private void OnCursorHoveredGenerator(ItemType type, int level)
        {
            var itemDescription = Translation.GetItemDescription(type, level);

            if (type == ItemType.TrashCan)
            {
                var instruction = "Перетащи сюда предмет, чтобы выбросить его.\nУлучши, чтобы можно было продавать предметы.";
                if (level > 2)
                    instruction = "Перетащи сюда предмет, чтобы продать его.\nУлучши, чтобы продажа приносила больше бриллиантов.";
                _informationWindow.ShowGenerator(itemDescription.Title, level, itemDescription.Description, null, instruction);
            }
            else
            {
                _informationWindow.ShowGenerator(itemDescription.Title, level,
                    itemDescription.Description, GetProducedItemSprites?.Invoke(type));
            }
        }
    }
}
