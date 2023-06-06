using System.Runtime.InteropServices;
using UnityEngine;
using YG;

namespace Service
{
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance { get; private set; }
        
        public Language Language { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        //private void Start()
        //{
        //    var language = GetLang();
        //    Language = language switch
        //    {
        //        "ru" => Language.Russian,
        //        _ => Language.English,
        //    };
        //    GameStorage.Instance.Language = Language;
        //}

        [DllImport("__Internal")]
        private static extern string GetLang();
    }
}