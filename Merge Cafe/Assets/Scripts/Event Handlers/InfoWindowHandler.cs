using Enums;
using Gameplay;
using Gameplay.Counters;
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
        [SerializeField] private Color _highlightColor;

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
            StarCounter.CursorHoveredItem += OnCursorHoveredItem;
            StarCounter.CursorLeftItem += _informationWindow.Hide;
        }

        private void OnDisable()
        {
            Item.CursorHoveredMovableItem -= OnCursorHoveredItem;
            Item.CursorHoveredNotMovableItem -= OnCursorHoveredItem;
            Item.CursorLeftItem -= _informationWindow.Hide;
            Upgradable.CursorHoveredGenerator -= OnCursorHoveredGenerator;
            Upgradable.CursorLeftGenerator -= _informationWindow.Hide;
            StarCounter.CursorHoveredItem -= OnCursorHoveredItem;
            StarCounter.CursorLeftItem -= _informationWindow.Hide;
        }

        private void OnCursorHoveredItem(ItemType type, int level)
        {
            var storage = GameStorage.Instance;
            var language = storage.Language;
            var highlightColor = ColorUtility.ToHtmlStringRGB(_highlightColor);
            var itemDescription = Translation.GetItemDescription(language, type, level);
            var instruction = "";
            var maxLevel = storage.IsItemMaxLevel(type, level);
            Translation.ItemDescription nextDescription = null;

            if (!maxLevel)
                nextDescription = Translation.GetItemDescription(language, type, level + 1);

            var isGenerator = IsGenerator?.Invoke(type);
            if (isGenerator.GetValueOrDefault())
            {
                var isGeneratorMaxLevel = IsGeneratorMaxLevel?.Invoke(type, level);
                if (isGeneratorMaxLevel.GetValueOrDefault())
                {
                    var parts = Translation.GetDragEquipUpgradeGeneratorInfoParts(language);
                    instruction = $"{parts[0]}<color=#{highlightColor}>«{itemDescription.Title}»</color>{parts[1]}.\n\n" +
                        $"{Translation.GetCannotBeThrownText(language)}.";
                }
                else
                {
                    var currentLevel = GetGeneratorLevel?.Invoke(type);
                    var titleNextLevel = Translation.GetItemDescription(language, type, level + 1).Title;
                    var titleNeedLevel = Translation.GetItemDescription(language, type, currentLevel.GetValueOrDefault()).Title;
                    var parts = Translation.GetMergeGeneratorInfoParts(language);
                    instruction = $"{parts[0]}<color=#{highlightColor}>«{titleNextLevel}»{parts[1]}{level + 1}{parts[2]}</color>.\n\n" +
                        $"{parts[3]}<color=#{highlightColor}>«{titleNeedLevel}»{parts[4]}{currentLevel.GetValueOrDefault()}{parts[5]}</color>{parts[6]}<color=#{highlightColor}>«{titleNeedLevel}»</color>.\n\n" +
                        $"{Translation.GetCannotBeThrownText(language)}.";
                }
            }
            else if (type == ItemType.Brilliant)
            {
                var currencyCount = storage.GetBrilliantsRewardByItemlevel(level);
                var brilliantWord = language == Language.Russian
                    ? Translation.PluralizeWord(currencyCount, "кристалл", "кристалла", "кристаллов")
                    : Translation.GetItemTitle(language, ItemType.Brilliant);
                var parts = Translation.GetMergeOrGetCurrencyInfoParts(language);
                if (maxLevel)
                    instruction = $"{parts[0]}<color=#{highlightColor}>{currencyCount} {brilliantWord}</color>.";
                else
                    instruction = $"{parts[0]}<color=#{highlightColor}>{currencyCount} {brilliantWord}</color>{parts[1]}.";
            }
            else if (type == ItemType.Star)
            {
                var currencyCount = storage.GetStarsRewardByItemLevel(level);
                var starWord = language == Language.Russian 
                    ? Translation.PluralizeWord(currencyCount, "звезду", "звезды", "звёзд")
                    : Translation.GetItemTitle(language, ItemType.Star);
                var parts = Translation.GetMergeOrGetCurrencyInfoParts(language);
                if (maxLevel)
                    instruction = $"{parts[0]}<color=#{highlightColor}>{currencyCount} {starWord}</color>.";
                else
                    instruction = $"{parts[0]}<color=#{highlightColor}>{currencyCount} {starWord}</color>{parts[1]}.";
            }
            else if (type == ItemType.Present)
            {
                var parts = Translation.GetMergeOrOpenPresentInfoParts(language);
                if (maxLevel)
                    instruction = $"{parts[0]}.";
                else
                    instruction = $"{parts[0]}{parts[1]}.";
            }
            else if (type == ItemType.OpenPresent)
                instruction = $"{Translation.GetFromPresentInfo(language)}!";
            else if (type == ItemType.Key)
            {
                var parts = Translation.GetDragKeyToLockInfoParts(language);
                if (maxLevel)
                    instruction = $"{parts[0]}<color=#{highlightColor}>{level}{parts[1]}</color>.";
                else
                    instruction = $"{parts[0]}<color=#{highlightColor}>{level}{parts[1]}</color>{parts[2]}.";
            }
            else if (type == ItemType.Lock)
            {
                var parts = Translation.GetHowRemoveLockInfoParts(language);
                instruction = $"{parts[0]}<color=#{highlightColor}>{level}{parts[1]}</color>.";
            }
            else if (type == ItemType.Box)
            {
                if (maxLevel)
                    instruction = $"{Translation.GetBoxInfoPart(language, 3)}.";
                else if (level == 2)
                    instruction = $"{Translation.GetBoxInfoPart(language, 2)}<color=#{highlightColor}>«{nextDescription.Title}»</color>.";
                else if (level == 1)
                    instruction = $"{Translation.GetBoxInfoPart(language, 1)}<color=#{highlightColor}>«{nextDescription.Title}»</color>.";
            }
            else if (type == ItemType.Energy)
            {
                var energyCount = GameStorage.Instance.GetEnergyRewardByItemlevel(level);
                var itemWord = language == Language.Russian
                    ? Translation.PluralizeWord(energyCount, "предмет", "предмета", "предметов")
                    : Translation.GetItemText(language);
                var parts = Translation.GetEnergyInfoParts(language);
                if (maxLevel)
                    instruction = $"{parts[0]}<color=#{highlightColor}>{energyCount} {itemWord}</color>).";
                else
                instruction = $"{parts[0]}<color=#{highlightColor}>{energyCount} {itemWord}</color>){parts[1]}.";


            }
            else if (type == ItemType.Duplicator)
            {
                instruction = $"{Translation.GetDuplicatorInfoPart(language, level)}.";
            }
            else if (type == ItemType.Doubler)
            {
                var count = level switch
                {
                    1 => 1,
                    2 => 4,
                    3 => 9,
                    _ => 1
                };
                var parts = Translation.GetDoublerInfoParts(language);
                if (language == Language.Russian)
                {
                    instruction = $"{parts[0]} <color=#{highlightColor}>{count} " +
                        $"{Translation.PluralizeWord(count, "заказа", "заказов", "заказов")}</color>.";
                }
                else
                {
                    instruction = $"{parts[0]} <color=#{highlightColor}>{count} {parts[1]}{(count > 1 ? "s" : "")}</color>.";
                }
            }
            else
            {
                var parts = Translation.GetOrdinaryItemInfoParts(language);
                if (maxLevel)
                    instruction = $"{parts[0]}<color=#{highlightColor}>«{itemDescription.Title}»</color>.";
                else
                    instruction = $"{parts[0]}«{itemDescription.Title}»{parts[1]}<color=#{highlightColor}>«{nextDescription.Title}»</color>.";
            }

            _informationWindow.ShowItem(itemDescription.Title, level, itemDescription.Description, instruction);
        }

        private void OnCursorHoveredGenerator(ItemType type, int level)
        {
            var itemDescription = Translation.GetItemDescription(GameStorage.Instance.Language, type, level);

            if (type == ItemType.TrashCan)
            {
                var parts = Translation.GetTrashCanInfoParts(GameStorage.Instance.Language);
                var instruction = $"{parts[0]}.\n\n{parts[1]}.";
                if (level > 2)
                    instruction = $"{parts[0]}.\n\n{parts[2]}.";
                _informationWindow.ShowGenerator(itemDescription.Title, level, itemDescription.Description, null, instruction);
            }
            else
            {
                var parts = Translation.GetUpgradeGeneratorInfoParts(GameStorage.Instance.Language);
                var instruction = $"{parts[0]}.";
                if (level == 1 || level == 4)
                    instruction = $"{parts[1]}.";
                instruction += $"\n\n{parts[2]}:";
                _informationWindow.ShowGenerator(itemDescription.Title, level,
                    itemDescription.Description, GetProducedItemSprites?.Invoke(type), instruction);
            }
        }
    }
}
