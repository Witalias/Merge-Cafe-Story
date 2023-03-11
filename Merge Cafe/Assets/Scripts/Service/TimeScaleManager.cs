using UnityEngine;

namespace Service
{
    public class TimeScaleManager : MonoBehaviour
    {
        public void Stop() => Time.timeScale = 0;

        public void SetNormal() => Time.timeScale = 1;
    }
}
