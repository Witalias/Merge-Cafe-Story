using Service;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(IStorable))]
    public class SaveHandler : MonoBehaviour
    {
        private IStorable _storable;

        public void Save() => _storable.Save();

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