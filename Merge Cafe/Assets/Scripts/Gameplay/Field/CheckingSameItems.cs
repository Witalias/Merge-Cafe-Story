using Enums;
using Service;
using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public class CheckingSameItems : MonoBehaviour
    {
        [SerializeField] private float _checkingDelay = 5f;
        [SerializeField] private Cell[] _cells;
        [SerializeField] private ItemTypeLevel[] _itemsCanBeCollected;

        public static event Action<ItemStorage> CheckTutorialItem;
        public static event Action<Transform, bool> PlayClickAnimationCursor;

        private void OnEnable()
        {
            StartCoroutine(Check());
        }

        private IEnumerator Check()
        {
            var waitFrame = new WaitForEndOfFrame();
            var waitDelay = new WaitForSeconds(_checkingDelay);
            while (true)
            {
                for (var i = 0; i < _cells.Length; ++i)
                {
                    if (!IsItem(i))
                        continue;

                    var firstItem = _cells[i].Item;
                    yield return waitFrame;

                    CheckTutorialItem?.Invoke(firstItem.Stats);

                    if (Array.Exists(_itemsCanBeCollected, 
                        item => item.Type == firstItem.Stats.Type && item.Level == firstItem.Stats.Level))
                    {
                        PlayClickAnimationCursor?.Invoke(firstItem.transform, false);
                        break;
                    }

                    if (firstItem == null
                        || GameStorage.Instance.IsItemMaxLevel(firstItem.Stats.Type, firstItem.Stats.Level) 
                        || firstItem.Stats.Type == ItemType.OpenPresent
                        || firstItem.AvailableForOrder)
                        continue;

                    for (var j = i + 1; j < _cells.Length; ++j)
                    {
                        if (!IsItem(j))
                            continue;
                        var secondItem = _cells[j].Item;
                        if (secondItem.EqualTo(firstItem))
                        {
                            firstItem.PlayHighlight();
                            secondItem.PlayHighlight();
                            yield return waitDelay;
                            StartCoroutine(Check());
                            yield break;
                        }
                    }
                }
                yield return waitDelay;
            }
        }

        private bool IsItem(int index) => !_cells[index].Empty && _cells[index].Item.Stats.Type != ItemType.Lock;
    }
}
