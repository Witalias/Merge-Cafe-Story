using System;
using System.Collections;
using UnityEngine;

namespace Service
{
    public class AutoSaveSystem : MonoBehaviour
    {
        [SerializeField] private float _saveInterval;

        public static Action Saved;

        private void Start()
        {
            StartCoroutine(Save());
        }

        private void OnApplicationQuit()
        {
            Saved?.Invoke();
        }

        private IEnumerator Save()
        {
            yield return new WaitForSeconds(0.1f);
            Saved?.Invoke();
            yield return new WaitForSeconds(_saveInterval);
            StartCoroutine(Save());
        }
    }
}