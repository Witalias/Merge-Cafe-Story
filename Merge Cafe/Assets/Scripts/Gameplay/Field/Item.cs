using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Service;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Animator))]
    public class Item : MonoBehaviour
    {
        private const string _zoomAnimatorBool = "Mouse Enter";
        private const string _burnAnimatorBool = "Burn";

        [SerializeField] private float returningSpeed;
        [SerializeField] private float followSpeed;
        [SerializeField] private Image _image;

        private Animator _animator;
        private Cell _currentCell;
        private Camera _mainCamera;
        private GameStorage _storage;

        private bool _isReturning = false;

        public ItemStats Stats { get; private set; }

        public void Initialize(ItemStats stats)
        {
            _image.sprite = stats.Icon;
            Stats = stats;

            _animator.SetTrigger(_burnAnimatorBool);
        }

        public void SetCell(Cell cell) => _currentCell = cell;

        public void ReturnToCell() => _isReturning = true;

        public bool EqualTo(Item other)
        {
            if (Stats.Type == other.Stats.Type)
                return Stats.Level == other.Stats.Level;
            return false;
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
                transform.position = Vector3.Lerp(transform.position, _currentCell.transform.position, returningSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, _currentCell.transform.position) <= 0.01f)
                    _isReturning = false;
            }
        }

        private void OnMouseEnter()
        {
            _animator.SetBool(_zoomAnimatorBool, true);
        }

        private void OnMouseExit()
        {
            _animator.SetBool(_zoomAnimatorBool, false);
        }

        private void OnMouseDrag()
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var followPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            transform.position = Vector3.Lerp(transform.position, followPosition, followSpeed * Time.fixedDeltaTime);
        }

        private void OnMouseDown()
        {
            _isReturning = false;
            transform.SetAsLastSibling();
        }

        private void OnMouseUp()
        {
            var cell = GetOverCell();
            ReturnToCell();
            if (cell == null)
                return;
            else if (cell.Empty)
                Move(cell);
            else if (EqualTo(cell.Item))
            {
                if (_currentCell == cell)
                    return;

                if (!_storage.IsItemMaxLevel(Stats))
                    Join(cell);
            }
            else
                Swap(cell);
        }

        private Cell GetOverCell()
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            foreach (var obj in raycastResults)
            {
                var cell = obj.gameObject.GetComponent<Cell>();
                if (obj.gameObject.GetComponent<Cell>() != null)
                    return cell;
            }
            return null;
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
            _currentCell.Clear();
            withCell.Clear();
            withCell.CreateItem(_storage.GetNextItemByAnotherItem(Stats));
            Destroy(gameObject);
        }
    }
}
