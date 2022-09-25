using UnityEngine;
using System.Collections;
using UI;
using Service;
using Enums;
using Gameplay.Field;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ItemGenerator : MonoBehaviour
{
    private const float _tick = 0.01f;
    private const string _zoomAnimatorBool = "Mouse Enter";
    private const string _clickAnimatorBool = "Click";

    [SerializeField] private Image _image;
    [SerializeField] private UIBar _bar;
    [SerializeField] private ItemType[] _generatedItems;
    [SerializeField] private GeneratorStats[] _statsOnLevels;

    private Animator _animator;
    private GameStorage _storage;
    private Coroutine _addBarValueCoroutine;

    private int _level = 1;
    private float _currentGenerationTime = 0f;
    private bool stopped = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _storage = GameStorage.Instanse;
        _image.sprite = _statsOnLevels[_level - 1].icon;
        _addBarValueCoroutine = StartCoroutine(AddBarValue());
    }

    private void Update()
    {
        if (stopped && _storage.GetFirstEmptyCell() != null)
        {
            stopped = false;
            _addBarValueCoroutine = StartCoroutine(AddBarValue());
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

    private void OnMouseDown()
    {
        if (stopped)
            return;

        _animator.SetTrigger(_clickAnimatorBool);
        _currentGenerationTime += _statsOnLevels[_level - 1].clickIncreaseSeconds;
        UpdateBar();
        CheckBarFilled();
    }

    private IEnumerator AddBarValue()
    {
        _currentGenerationTime += _tick;
        UpdateBar();

        yield return new WaitForSeconds(_tick);

        CheckBarFilled();
        if (!stopped)
            _addBarValueCoroutine = StartCoroutine(AddBarValue());
    }

    private void UpdateBar()
    {
        _bar.SetValue(_currentGenerationTime / _statsOnLevels[_level - 1].generationTimeInSec * 100f);
    }

    private void CheckBarFilled()
    {
        var reachedTime = _statsOnLevels[_level - 1].generationTimeInSec;
        if (_currentGenerationTime >= reachedTime)
        {
            CreateItem();
            if (!stopped)
                _currentGenerationTime -= reachedTime;
        }
    }

    private void CreateItem()
    {
        var cell = _storage.GetFirstEmptyCell();
        if (cell == null)
        {
            StopCoroutine(_addBarValueCoroutine);
            stopped = true;
            return;
        }
        var randomType = _generatedItems[Random.Range(0, _generatedItems.Length)];
        var randomLevel = Random.Range(1, _statsOnLevels[_level - 1].initItemsLevel + 1);
        var item = new ItemStats(randomLevel, _storage.GetItemSprite(randomType, randomLevel), randomType);
        cell.CreateItem(item, transform.position);
    }
}

[System.Serializable]
public class GeneratorStats
{
    public Sprite icon;
    public float generationTimeInSec;
    public float clickIncreaseSeconds;
    public int initItemsLevel;
}