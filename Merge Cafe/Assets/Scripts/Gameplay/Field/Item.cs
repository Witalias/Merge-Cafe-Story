using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Service;
using System;
using Enums;
using Gameplay.Orders;
using System.Linq;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Animator))]
    public class Item : MonoBehaviour
    {
        private const string _zoomAnimatorBool = "Mouse Enter";
        private const string _burnAnimatorTrigger = "Burn";

        [SerializeField] private float _returningSpeed;
        [SerializeField] private float _followSpeed;
        [SerializeField] private Image _image;

        private Animator _animator;
        private Cell _currentCell;
        private Camera _mainCamera;
        private GameStorage _storage;

        private bool _isReturning = false;

        public static event Action JoiningItemsOfMaxLevelTried;
        public static event Action<ItemType> CursorHoveredItem;

        public ItemStats Stats { get; private set; }

        public void Initialize(ItemStats stats)
        {
            _image.sprite = stats.Icon;
            Stats = stats;

            _animator.SetTrigger(_burnAnimatorTrigger);
        }

        public void SetCell(Cell cell) => _currentCell = cell;

        public void ReturnToCell() => _isReturning = true;

        public bool EqualTo(Item other) => Stats.EqualTo(other.Stats);

        public GameObject[] GetScreenRaycastResults()
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            return raycastResults.Select(element => element.gameObject).ToArray();
        }

        public void OpenPresent()
        {
            var openPresentStats = new ItemStats(Stats.Level, _storage.GetItemSprite(ItemType.OpenPresent, Stats.Level), ItemType.OpenPresent);
            Remove();
            _currentCell.CreateItem(openPresentStats);
        }

        public void Remove()
        {
            _currentCell.Clear();
            Destroy(gameObject);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mainCamera = Camera.main;
            _storage = GameStorage.Instanse;
        }

        private void Update()
        {
            if (_isReturning)
            {
                transform.position = Vector3.Lerp(transform.position, _currentCell.transform.position, _returningSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, _currentCell.transform.position) <= 0.01f)
                    _isReturning = false;
            }
        }

        private void OnMouseEnter()
        {
            _animator.SetBool(_zoomAnimatorBool, true);
            CursorHoveredItem?.Invoke(Stats.Type);
        }

        private void OnMouseExit()
        {
            _animator.SetBool(_zoomAnimatorBool, false);
        }

        private void OnMouseDrag()
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var followPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            transform.position = Vector3.Lerp(transform.position, followPosition, _followSpeed * Time.fixedDeltaTime);
        }

        private void OnMouseDown()
        {
            _isReturning = false;
            transform.SetAsLastSibling();
        }

        private void OnMouseUp()
        {
            ReturnToCell();
            CheckCursorOver();
        }

        private void CheckCursorOver()
        {
            var hits = GetScreenRaycastResults();
            foreach (var obj in hits)
            {
                var cell = obj.GetComponent<Cell>();
                if (cell != null)
                    InteractWithCell(cell);
                else
                {
                    var order = obj.GetComponent<Order>();
                    if (order != null)
                        InteractWithOrder(order);
                    else
                    {

                    }
                }

            }
        }

        private void InteractWithOrder(Order order)
        {
            if (order == null)
                return;

            order.CheckIncomingItem(Stats, out bool matches);
            if (!matches)
                return;

            _currentCell.Clear();
            Destroy(gameObject);
        }

        private void InteractWithCell(Cell cell)
        {
            if (cell == null)
                return;
            else if (cell.Empty)
                Move(cell);
            else if (EqualTo(cell.Item))
            {
                if (_currentCell == cell)
                    return;

                if (_storage.IsItemMaxLevel(Stats))
                    JoiningItemsOfMaxLevelTried?.Invoke();
                else
                    Join(cell);
            }
            else
                Swap(cell);
        }

        private void Move(Cell toCell)
        {
            _currentCell.Clear();
            toCell.SetItem(this);
            _currentCell = toCell;
        }

        private void Swap(Cell withCell)
        {
            _currentCell.Clear();
            _currentCell.SetItem(withCell.Item);
            withCell.Item.ReturnToCell();
            withCell.Clear();
            withCell.SetItem(this);
            _currentCell = withCell;
        }

        private void Join(Cell withCell)
        {
            Destroy(withCell.Item.gameObject);
            withCell.Clear();
            withCell.CreateItem(_storage.GetNextItemByAnotherItem(Stats));
            Remove();
        }
    }
}
