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
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            _dropdown.value = (int)GameStorage.Instance.Language;
        }

        private void OnValueChanged(int index)
        {
            GameStorage.Instance.Language = (Language)index;
            LanguageChanged?.Invoke();
        }
    }
}
