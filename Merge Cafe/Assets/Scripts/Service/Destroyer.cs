using UnityEngine;
using System.Collections;

namespace Service
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] private float _destroyIn = 0f;

        public void Remove() => Destroy(gameObject);

        private void Awake()
        {
            if (_destroyIn > 0f)
                StartCoroutine(RemoveIn());
        }

        private IEnumerator RemoveIn()
        {
            yield return new WaitForSeconds(_destroyIn);
            Remove();
        }
    }
}
