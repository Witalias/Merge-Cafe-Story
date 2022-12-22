using Service;
using System.Threading.Tasks;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(IStorable))]
    public class SaveHandler : MonoBehaviour
    {
        private IStorable _storable;

        public async void Save()
        {
            _storable.Save();
            await Task.Yield();
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