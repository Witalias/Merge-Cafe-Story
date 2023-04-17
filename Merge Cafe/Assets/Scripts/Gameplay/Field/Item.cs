using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Service;
using System;
using Enums;
using Gameplay.Orders;
using Gameplay.ItemGenerators;
using Gameplay.Tutorial;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(QuickClickTracking))]
    public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        private const string _zoomAnimatorBool = "Mouse Enter";
        private const string _burnAnimatorTrigger = "Burn";
        private const string _disappearAnimatorTrigger = "Disappear";
        private const string _highlightAnimatorTrigger = "Highlight";

        [SerializeField] private float _returningSpeed;
        [SerializeField] private float _followSpeed;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _highlightParticlePrefab;
        [SerializeField] private GameObject _mergeParticlePrefab;

        private Animator _animator;
        private Cell _currentCell;
        private Camera _mainCamera;
        private GameStorage _storage;
        private QuickClickTracking _quickClickTracking;

        private bool _isReturning = false;
        private bool _dragged = false;

        public static event Action MergingItemsOfMaxLevelTried;
        public static event Action<ItemType, int> CursorHoveredMovableItem;
        public static event Action<ItemType, int> CursorHoveredNotMovableItem;
        public static event Action CursorLeftItem;
        public static event Action CannotBeThrownAway;
        public static event Action WrongLevelForCombinating;
        public static event Action<int, int> CellChanged;
        public static event Action<int> ItemRemoved;
        public static event Action ItemCaptured;
        public static event Action ItemsMerged;
        public static event Action ItemsCombinated;

        public ItemStorage Stats { get; private set; }

        public Cell CurrentCell { get => _currentCell; }

        public void Initialize(ItemStorage stats)
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
            var openPresentStats = new ItemStorage(_storage.GetItem(ItemType.OpenPresent, Stats.Level));
            Remove();
            _currentCell.CreateItem(openPresentStats);
        }

        public void Remove()
        {
            ItemRemoved?.Invoke(CurrentCell.transform.GetSiblingIndex());
            _currentCell.Clear();
            Destroy(gameObject);
        }

        public void SetActiveParticles(bool value) => _highlightParticlePrefab.SetActive(value);

        public void PlayHighlight() => _animator.SetTrigger(_highlightAnimatorTrigger);

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!TutorialSystem.TutorialDone && GetComponent<TutorialTarget>() == null)
                return;

            ItemCaptured?.Invoke();
            _isReturning = false;
            transform.SetAsLastSibling();
            SoundManager.Instanse.Play(_storage.GetItem(Stats.Type, Stats.Level).TakeSound, null);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragged = false;
            ReturnToCell();

            if (!Stats.Movable || (!TutorialSystem.TutorialDone && GetComponent<TutorialTarget>() == null))
                return;

            CheckCursorOver();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Stats.Movable)
            {
                _animator.SetBool(_zoomAnimatorBool, true);
                if (!_dragged)
                    CursorHoveredMovableItem?.Invoke(Stats.Type, Stats.Level);
                Stats.Unlock();
            }
            else if (!_dragged)
                CursorHoveredNotMovableItem?.Invoke(Stats.Type, Stats.Level);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorLeftItem?.Invoke();

            if (!Stats.Movable)
                return;

            _animator.SetBool(_zoomAnimatorBool, false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragged = true;

            if (_quickClickTracking.IsChecking || !Stats.Movable ||
                (!TutorialSystem.TutorialDone && GetComponent<TutorialTarget>() == null))
                return;

            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var followPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            transform.position = Vector3.Lerp(transform.position, followPosition, _followSpeed * Time.fixedDeltaTime);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _quickClickTracking = GetComponent<QuickClickTracking>();
            _mainCamera = Camera.main;
            _storage = GameStorage.Instance;
            SetActiveParticles(false);
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

        private IEnumerator Disappear()
        {
            _isReturning = false;
            _animator.SetTrigger(_disappearAnimatorTrigger);
            //_currentCell.Clear();
            yield return new WaitForSeconds(0.5f);
            Remove();
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
                        var upgraded = false;
                        var upgradable = obj.GetComponent<Upgradable>();
                        if (upgradable != null)
                            InteractWithUpgradableObject(upgradable, out upgraded);

                        if (!upgraded)
                        {
                            var trashCan = obj.GetComponent<TrashCan>();
                            if (trashCan != null)
                                InteractWithTrashCan(trashCan);
                        }
                    }
                }

            }
        }

        private void InteractWithUpgradableObject(Upgradable obj, out bool upgraded)
        {
            if (obj.CheckIncomingItem(Stats))
            {
                upgraded = true;
                Remove();
            }
            else
                upgraded = false;
        }

        private void InteractWithTrashCan(TrashCan trashCan)
        {
            if (trashCan == null || (!TutorialSystem.TutorialDone && GetComponent<TutorialExtraTarget>() == null))
                return;

            if (trashCan.GetComponent<Upgradable>().CheckIncomingItem(Stats))
                return;

            if (Stats.Throwable)
            {
                trashCan.Throw(Stats.Level);
                StartCoroutine(Disappear());
            }
            else
                CannotBeThrownAway?.Invoke();
        }

        private void InteractWithOrder(Order order)
        {
            if (order == null)
                return;

            order.CheckIncomingItem(Stats, out bool matches);
            if (!matches)
                return;

            Remove();
        }

        private void InteractWithCell(Cell cell)
        {
            if (cell == null || _currentCell == cell)
            {
                //SoundManager.Instanse.Play(_storage.GetItem(Stats.Type, Stats.Level).PutSound, null);
                return;
            }
            else if (cell.Empty)
            {
                Move(cell);
            }
            else if (EqualTo(cell.Item) && cell.Item.Stats.Type != ItemType.OpenPresent)
            {
                if (_storage.IsItemMaxLevel(Stats.Type, Stats.Level))
                    MergingItemsOfMaxLevelTried?.Invoke();
                else
                    Join(cell);
            }
            else if (_storage.IsCombinatingWith(Stats.Type, cell.Item.Stats.Type))
            {
                if (Stats.Level == cell.Item.Stats.Level)
                    Combinate(cell);
                else
                    WrongLevelForCombinating?.Invoke();
            }
            else if (Stats.Type == ItemType.Duplicator
                && cell.Item.Stats.Type != ItemType.Lock
                && cell.Item.Stats.Type != ItemType.Duplicator)
            {
                Duplicate(cell);
            }
            else if (Stats.Movable && cell.Item.Stats.Movable)
                Swap(cell);
        }

        private void Move(Cell toCell)
        {
            if (!TutorialSystem.TutorialDone)
                return;

            CellChanged?.Invoke(_currentCell.transform.GetSiblingIndex(), toCell.transform.GetSiblingIndex());
            _currentCell.Clear();
            toCell.SetItem(this);
            _currentCell = toCell;
            SoundManager.Instanse.Play(_storage.GetItem(Stats.Type, Stats.Level).PutSound, null);
        }

        private void Swap(Cell withCell)
        {
            if (!TutorialSystem.TutorialDone)
                return;

            CellChanged?.Invoke(_currentCell.transform.GetSiblingIndex(), withCell.transform.GetSiblingIndex());
            _currentCell.Clear();
            _currentCell.SetItem(withCell.Item);
            withCell.Item.ReturnToCell();
            withCell.Clear();
            withCell.SetItem(this);
            _currentCell = withCell;
            SoundManager.Instanse.Play(_storage.GetItem(Stats.Type, Stats.Level).PutSound, null);
        }

        private void Join(Cell withCell)
        {
            SoundManager.Instanse.Play(Sound.Merge, null, Stats.Level - 1);
            Destroy(withCell.Item.gameObject);
            withCell.Clear();
            withCell.CreateItem(_storage.GetNextItemByAnotherItem(Stats));
            Remove();

            if (Stats.Level >= _storage.GenerationStarFromLevel)
            {
                var randomCell = _storage.GetRandomEmptyCell();
                randomCell.CreateItem(_storage.GetItem(ItemType.Star, 1), transform.position);
            }
            Instantiate(_mergeParticlePrefab, withCell.transform.position, Quaternion.identity);
            ItemsMerged?.Invoke();
        }

        private void Combinate(Cell withCell)
        {
            _isReturning = false;
            SoundManager.Instanse.Play(_storage.GetCombinateSound(Stats.Type, withCell.Item.Stats.Type), null);
            StartCoroutine(Disappear());
            StartCoroutine(withCell.Item.Disappear());
            ItemsCombinated?.Invoke();
        }

        private void Duplicate(Cell withCell)
        {
            SoundManager.Instanse.Play(Sound.Merge, null, Stats.Level - 1);
            var itemMaxLevel = GameStorage.Instance.GetItemMaxLevel(withCell.Item.Stats.Type);
            var duplicatorNormalLevel = 2;
            Remove();
            var duplicatedItemLevel = withCell.Item.Stats.Level - (duplicatorNormalLevel - Stats.Level);
            
            if (duplicatedItemLevel < 1)
                duplicatedItemLevel = 1;

            if (duplicatedItemLevel > itemMaxLevel)
                duplicatedItemLevel = itemMaxLevel;

            _currentCell.CreateItem(GameStorage.Instance.GetItem(withCell.Item.Stats.Type, duplicatedItemLevel));
        }
    }
}
