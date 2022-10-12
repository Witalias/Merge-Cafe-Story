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

        private GameStorage _storage;

        private bool _busy = false;
        private ItemType _currentType;

        public void Show(ItemType type)
        {
            if (type == ItemType.OpenPresent)
                return;

            if (_busy && type != _currentType)
                return;

            _currentType = type;
            var length = GetLength(type);

            for (var i = 0; i < length; ++i)
            {
                if (i > 0)
                    _arrows[i - 1].SetActive(true);

                var item = _storage.GetItem(type, i + 1);
                if (item == null)
                {
                    Hide();
                    return;
                }
                _icons[i].gameObject.SetActive(true);
                _icons[i].SetSprite(item.Unlocked ? item.Icon : _storage.QuestionMark);

                if (item.IsNew && item.Unlocked)
                {
                    item.NotNew();
                    _icons[i].PayAttentionAnimation();
                    _icons[i].ShowReward(i + 1);
                    _busy = true;
                }
            }
        }

        public void ShowCombination(ItemType movable, ItemType destroyable)
        {
            if (_busy)
                return;

            var length = _storage.GetItemMaxLevel(movable);
            for (var i = 0; i < length * 2; i += 2)
            {
                var item = _storage.GetItem(movable, i / 2 + 1);
                _icons[i].gameObject.SetActive(true);
                _icons[i].SetSprite(item.Unlocked ? item.Icon : _storage.QuestionMark);
                _arrows[i].SetActive(true);
                _icons[i + 1].gameObject.SetActive(true);
                _icons[i + 1].SetSprite(_storage.GetItemSprite(destroyable, i / 2 + 1));
                _icons[i + 1].SetCrossActive(true);
            }
        }

        public void Hide()
        {
            if (_busy)
                return;

            foreach (var icon in _icons)
            {
                icon.SetCrossActive(false);
                icon.gameObject.SetActive(false);
            }
            foreach (var arrow in _arrows)
                arrow.SetActive(false);
        }

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;
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

        private int GetLength(ItemType type) => Mathf.Min(_storage.GetItemMaxLevel(type), _icons.Length);
    }
}
