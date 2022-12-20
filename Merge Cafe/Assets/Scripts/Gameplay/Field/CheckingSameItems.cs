using Enums;
using Service;
using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public class CheckingSameItems : MonoBehaviour
    {
        [SerializeField] private float _checkingDelay = 5f;
        [SerializeField] private Cell[] _cells;

        private void OnEnable()
        {
            StartCoroutine(Check());
        }

        private IEnumerator Check()
        {
            for (var i = 0; i < _cells.Length; ++i)
            {
                if (!IsItem(i))
                    continue;
                var firstItem = _cells[i].Item;
                if (GameStorage.Instanse.IsItemMaxLevel(firstItem.Stats.Type, firstItem.Stats.Level))
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
                        yield return new WaitForSeconds(_checkingDelay);
                        StartCoroutine(Check());
                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(_checkingDelay);
            StartCoroutine(Check());
        }

        private bool IsItem(int index) => !_cells[index].Empty && _cells[index].Item.Stats.Type != ItemType.Lock;
    }
}
