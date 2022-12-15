using Service;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(IStorable))]
    public class SaveHandler : MonoBehaviour
    {
        private IStorable _storable;

        private void Awake()
        {
            _storable = GetComponent<IStorable>();
        }

        private void OnEnable()
        {
            AutoSaveSystem.Saved += _storable.Save;
        }

        private void OnDisable()
        {
            AutoSaveSystem.Saved -= _storable.Save;
        }
    }
}