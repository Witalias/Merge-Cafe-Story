using UnityEngine;

namespace CameraEngine
{
    public class CameraBounds : MonoBehaviour
    {
        [SerializeField] private float topLimit;
        [SerializeField] private float bottomLimit;
        [SerializeField] private float leftLimit;
        [SerializeField] private float rightLimit;

        public float TopLimit { get => topLimit; }

        public float BottomLimit { get => bottomLimit; }

        public float LeftLimit { get => leftLimit; }

        public float RightLimit { get => rightLimit; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(leftLimit, topLimit), new Vector2(rightLimit, topLimit));
            Gizmos.DrawLine(new Vector2(leftLimit, bottomLimit), new Vector2(rightLimit, bottomLimit));
            Gizmos.DrawLine(new Vector2(leftLimit, topLimit), new Vector2(leftLimit, bottomLimit));
            Gizmos.DrawLine(new Vector2(rightLimit, topLimit), new Vector2(rightLimit, bottomLimit));
        }
    }
}
