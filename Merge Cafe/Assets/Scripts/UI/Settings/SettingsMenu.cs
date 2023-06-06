using Service;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _sound;
        [SerializeField] private TextMeshProUGUI _music;
        [SerializeField] private TextMeshProUGUI _language;
        [SerializeField] private TextMeshProUGUI _reset;

        private void Start()
        {
            UpdateTexts();
        }

        public void UpdateTexts()
        {
            StartCoroutine(Do());
            IEnumerator Do()
            {
                yield return new WaitForEndOfFrame();
                var language = GameStorage.Instance.Language;
                _title.text = Translation.GetSettingsText(language);
                _sound.text = Translation.GetSoundsText(language);
                _music.text = Translation.GetMusicText(language);
                _language.text = Translation.GetLanguageText(language);
                _reset.text = Translation.GetResetText(language);
            }
        }
    }
}