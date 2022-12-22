using Service;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(Animation))]
    public class AutoSaveIconHandler : MonoBehaviour
    {
        private Animation _animation;

        private void OnEnable()
        {
            AutoSaveSystem.Saved += PlayAnimation;
        }

        private void OnDisable()
        {
            AutoSaveSystem.Saved -= PlayAnimation;
        }

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        private void PlayAnimation()
        {
            gameObject.SetActive(true);
            _animation.Play();
        }
    }
}
