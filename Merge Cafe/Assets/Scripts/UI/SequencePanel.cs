using UnityEngine;
using UnityEngine.UI;
using Enums;
using Service;

namespace UI
{
    public class SequencePanel : MonoBehaviour
    {
        [SerializeField] private ItemInSequence[] _icons;
        [SerializeField] private GameObject[] _arrows;

        private bool _busy = false;

        public void Show(ItemType type)
        {
            if (_busy)
                return;

            var storage = GameStorage.Instanse;
            var length = Mathf.Min(storage.GetItemMaxLevel(type), _icons.Length);

            for (var i = 0; i < length; ++i)
            {
                if (i > 0)
                    _arrows[i - 1].SetActive(true);

                var item = storage.GetItem(type, i + 1);
                if (item == null)
                {
                    Hide();
                    return;
                }
                _icons[i].gameObject.SetActive(true);
                _icons[i].SetSprite(item.Unlocked ? item.Icon : storage.QuestionMark);

                if (item.IsNew && item.Unlocked)
                {
                    item.NotNew();
                    _icons[i].PayAttentionAnimation();
                    _icons[i].ShowPresent(i + 1);
                    _busy = true;
                }
            }
        }

        public void Hide()
        {
            if (_busy)
                return;

            foreach (var icon in _icons)
                icon.gameObject.SetActive(false);
            foreach (var arrow in _arrows)
                arrow.SetActive(false);
        }

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (_busy)
            {
                _busy = false;
                for (var i = 0; i < _icons.Length; ++i)
                {
                    if (!_icons[i].gameObject.activeSelf)
                        break;

                    if (_icons[i].ContainsPresent)
                    {
                        _busy = true;
                        break;
                    }
                }
            }
        }
    }
}
