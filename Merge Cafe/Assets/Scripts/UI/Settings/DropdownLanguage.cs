using Service;
using System;
using TMPro;
using UnityEngine;

namespace UI.Settings
{
    public class DropdownLanguage : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;

        public static event Action LanguageChanged;

        private void Awake()
        {

        }

        private void Start()
        {
            InitializeLanguage();
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int index)
        {
            SetLanguage((Language)index);
            LanguageChanged?.Invoke();
        }

        private void InitializeLanguage()
        {
            //YG.YandexGame.LoadLocal();
            //var languageYG = YG.YandexGame.savesData.language;
            //switch (languageYG)
            //{
            //    case "en": SetLanguage(Language.English); break;
            //    case "ru": SetLanguage(Language.Russian); break;
            //}
            _dropdown.value = (int)GameStorage.Instance.Language;
        }

        private void SetLanguage(Language value) => GameStorage.Instance.Language = value;
    }
}
