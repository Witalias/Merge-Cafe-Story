using UnityEngine;
using Service;
using Enums;
using System.Collections;

namespace Gameplay.Counters
{
    public class CurrencyAdder : MonoBehaviour
    {
        [SerializeField] private float _changingCounterDelay = 2.5f;
        private Transform _mainCanvas;

        public static event System.Action<int> StarsChanged;
        public static event System.Action<int> BrilliantsChanged;

        private void Start()
        {
            _mainCanvas = GameObject.FindGameObjectWithTag(Tags.MainCanvas.ToString()).transform;
        }

        public void Add(CurrencyType type, int count, Vector2 animationStartPosition)
        {
            if (count > 0)
            {
                switch (type)
                {
                    case CurrencyType.Star:
                        Spawn(GameStorage.Instanse.StarForAnimation, count / 3 + 1, animationStartPosition);
                        break;
                    case CurrencyType.Brilliant:
                        Spawn(GameStorage.Instanse.BrilliantForAnimation, count / 20 + 1, animationStartPosition);
                        break;
                }
            }
            StartCoroutine(ChangeCounter(type, count));
        }

        private void Spawn(GameObject obj, int count, Vector2 position)
        {
            var leftTop = new Vector2(position.x - 0.2f, position.y + 0.2f);
            var rightBottom = new Vector2(position.x + 0.2f, position.y - 0.2f);
            for (var i = 0; i < count; ++i)
            {
                var randomX = Random.Range(leftTop.x, rightBottom.x);
                var randomY = Random.Range(leftTop.y, rightBottom.y);
                var spawnPoint = new Vector2(randomX, randomY);
                Instantiate(obj, spawnPoint, Quaternion.identity, _mainCanvas);
            }
        }

        private IEnumerator ChangeCounter(CurrencyType type, int count)
        {
            yield return new WaitForSeconds(count > 0 ? _changingCounterDelay : 0f);
            switch (type)
            {
                case CurrencyType.Star:
                    StarsChanged?.Invoke(count);
                    break;
                case CurrencyType.Brilliant:
                    BrilliantsChanged?.Invoke(count);
                    break;
            }
        }
    }
}