using Service;
using TMPro;
using UnityEngine;

namespace UI.Settings
{
    public class DropdownLanguage : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int index)
        {
            GameStorage.Instance.Language = (Language)index;
        }
    }
}
