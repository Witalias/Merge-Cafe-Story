using UnityEngine;
using UnityEngine.UI;
using Enums;
using Service;

namespace UI
{
    public class SequencePanel : MonoBehaviour
    {
        private const string _attentionAnimatorTrigger = "Attention";

        [SerializeField] private Image[] _icons;
        [SerializeField] private GameObject[] _arrows;

        public void Show(ItemType type)
        {
            var storage = GameStorage.Instanse;
            var length = Mathf.Min(storage.GetItemMaxLevel(type), _icons.Length);

            for (var i = 0; i < length; ++i)
            {
                if (i > 0)
                    _arrows[i - 1].SetActive(true);

                var item = storage.GetItem(type, i + 1);
                _icons[i].gameObject.SetActive(true);
                _icons[i].sprite = item.Unlocked ? item.Icon : storage.QuestionMark;

                if (item.IsNew && item.Unlocked)
                {
                    item.NotNew();
                    _icons[i].GetComponent<Animator>().SetTrigger(_attentionAnimatorTrigger);
                }
            }
        }

        public void Hide()
        {
            foreach (var icon in _icons)
                icon.gameObject.SetActive(false);
            foreach (var arrow in _arrows)
                arrow.SetActive(false);
        }

        private void Awake()
        {
            Hide();
        }
    }
}
