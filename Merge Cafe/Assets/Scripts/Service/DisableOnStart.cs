using UnityEngine;

namespace Service
{
    public class DisableOnStart : MonoBehaviour
    {
        [SerializeField] private GameObject _object;

        private void Awake()
        {
            _object.SetActive(true);
        }

        private void Start()
        {
            _object.SetActive(false);
        }
    }
}
