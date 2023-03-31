using EventHandlers;
using Gameplay.Tutorial;
using System;
using System.Collections;
using UnityEngine;

namespace Service
{
    public class AutoSaveSystem : MonoBehaviour
    {
        [SerializeField] private float _saveInterval;

        //private SaveHandler[] _saveHandlers;

        public static Action Saved;

        public void Save()
        {
            Saved?.Invoke();
        }

        private void Start()
        {
            //_saveHandlers = GameObject.FindObjectsOfType<SaveHandler>();
            StartCoroutine(SaveWithDelay());
        }

        private void OnApplicationQuit()
        {
            if (TutorialSystem.TutorialDone)
                Save();
        }

        private IEnumerator SaveWithDelay()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (TutorialSystem.TutorialDone)
                    Save();
                yield return new WaitForSeconds(_saveInterval);
            }
        }

        //private void Save()
        //{
        //    //var _saveHandlers = GameObject.FindObjectsOfType<SaveHandler>();
        //    foreach (var handler in _saveHandlers)
        //        handler.Save();
        //}
    }
}