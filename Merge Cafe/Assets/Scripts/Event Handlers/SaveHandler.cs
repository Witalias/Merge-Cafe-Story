using Gameplay.Tutorial;
using Service;
using System.Collections;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(IStorable))]
    public class SaveHandler : MonoBehaviour
    {
        private IStorable _storable;

        public void Save()
        {
            if (!TutorialSystem.TutorialDone)
                return;
            StartCoroutine(SaveCoroutine());
            IEnumerator SaveCoroutine()
            {
                yield return new WaitForSeconds(Random.Range(0f, 1f));
                _storable.Save();
            }
        }

        private void Awake()
        {
            _storable = GetComponent<IStorable>();
        }

        private void OnEnable()
        {
            AutoSaveSystem.Saved += Save;
        }

        private void OnDisable()
        {
            AutoSaveSystem.Saved -= Save;
        }
    }
}