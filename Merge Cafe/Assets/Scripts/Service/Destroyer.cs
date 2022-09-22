using UnityEngine;

namespace Service
{
    public class Destroyer : MonoBehaviour
    {
        public void Remove() => Destroy(gameObject);
    }
}
