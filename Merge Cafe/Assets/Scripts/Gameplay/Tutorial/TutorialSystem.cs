using Enums;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Gameplay.Tutorial
{
    public class TutorialSystem : MonoBehaviour, IStorable
    {
        private const string TUTORIAL_DONE_KEY = "TUTORIAL_DONE";
        private const string TUTORIAL_ITEM_TYPE_KEY = "TUTORIAL_ITEM_TYPE_";
        private const string TUTORIAL_ITEM_LEVEL_KEY = "TUTORIAL_ITEM_LEVEL_";
        private const string TUTORIAL_ITEMS_COUNT_KEY = "TUTORIAL_ITEMS_COUNT";
        private const string TUTORIAL_GENERATOR_SWITCH_COMPLETED_KEY = "TUTORIAL_GENERATOR_SWITCH_COMPLETED";

        [SerializeField] private Transform _rightTopSpawnWindowPoint;
        [SerializeField] private bool _skipStartTutorial;

        public static bool TutorialDone { get; private set; } = false;

        public static bool CanRandomOrders { get; private set; } = true;

        private GameStorage _storage;
        private TutorialStage _currentStage = TutorialStage.None;
        private GameObject _target = null;
        private Dictionary<(ItemType, int), Action> _conditionsForTutorial;
        private List<(ItemType, int)> _tutorialItems = new();
        private Dictionary<Language, Dictionary<TutorialStage, TutorialText>> _tutorialTexts = new()
        {
            [Language.English] = new()
            {
                [TutorialStage.Step1ClickTeapot] = new TutorialText("Let's get started!", "Make some tea."),
                [TutorialStage.Step2ContinueClickTeapot] = new TutorialText("Great!", "Keep clicking until the bar fills up."),
                [TutorialStage.Step3ExecuteFirstOrder] = new TutorialText("Complete the order!", "Drag the tea leaf to order."),
                [TutorialStage.Step4GetSequenceReward] = new TutorialText("What's shining there?", "You have completed the first order! Get a reward!"),
                [TutorialStage.Step5MergeCrystalls] = new TutorialText("Crystal!", "Merge crystals to get more!"),
                [TutorialStage.Step6CollectCrystalls] = new TutorialText("More crystals!", "Collect them!"),
                [TutorialStage.Step7_1GetFirstTeaLeaf] = new TutorialText("New order!", "You need a tea bag. First let's get 2 tea leaves."),
                [TutorialStage.Step7_2GetSecondTeaLeaf] = new TutorialText("New order!", "You need a tea bag. First let's get 2 tea leaves."),
                [TutorialStage.Step8MergeTeaLeafs] = new TutorialText("Merge!", "You know what to do."),
                [TutorialStage.Step9ExecuteSecondOrder] = new TutorialText("Done!", "Merge items to unlock new ones! Now complete the order."),
                [TutorialStage.Step10ThrowTrash] = new TutorialText("Ugh!", "The customer left the garbage. Throw it away!"),
                [TutorialStage.Step11GoToDecoreMode] = new TutorialText("Shopping time!", "Let's decorate the cafe."),
                [TutorialStage.Step12RemoveBoxes] = new TutorialText("What a mess!", "Put the boxes away."),
                [TutorialStage.Step13StartDialog] = new TutorialText("Talk!", "Listen to what the characters are talking about."),
                [TutorialStage.Step14ReturnToMainScreen] = new TutorialText("Customers are waiting!", "Let's go back to the orders."),
                [TutorialStage.Step15End] = new TutorialText("That's it!", "Fulfill orders, receive crystals, equip the cafe and try to MERGE EVERYTHING possible!"),
                [TutorialStage.StepKey1MoveKeyToLock] = new TutorialText("The key to happiness!", "Unlock an additional cell."),
                [TutorialStage.StepKey2End] = new TutorialText("Great!", "Now you have more free space!"),
                [TutorialStage.StepPresent1OpenPresent] = new TutorialText("Present!", "It contains useful items. Open it!"),
                [TutorialStage.StepPresent2GetContains] = new TutorialText("What's inside?", "Look what's in there!"),
                [TutorialStage.StepSwitch1] = new TutorialText("Shutdown", "As the equipment expands, you may need to temporarily disable unnecessary ones."),
                [TutorialStage.StepUpgrade1UpgradeGenerator] = new TutorialText("Upgrade time!", "How about merging the kettle?"),
                [TutorialStage.StepUpgrade2End] = new TutorialText("Level up!", "Now the kettle works faster!"),
                [TutorialStage.StepEnergy1MoveEnergyToGenerator] = new TutorialText("Energy!", "Speed up the kettle!"),
                [TutorialStage.StepBox1OpenBox] = new TutorialText("Box!", "It contains an item from the order. Try our luck?"),
                [TutorialStage.StepBox2End] = new TutorialText("Great!", "It looks like this is what we need. And what happens if we merge boxes? :)"),
            },
            [Language.Russian] = new()
            {
                [TutorialStage.Step1ClickTeapot] = new TutorialText("Начнём!", "Завари чай."),
                [TutorialStage.Step2ContinueClickTeapot] = new TutorialText("Отлично!", "Продолжай кликать до заполнения полосы."),
                [TutorialStage.Step3ExecuteFirstOrder] = new TutorialText("Выполни заказ!", "Перетащи чайный лист на заказ."),
                [TutorialStage.Step4GetSequenceReward] = new TutorialText("Что там блестит?", "Первый заказ выполнен! Забери награду!"),
                [TutorialStage.Step5MergeCrystalls] = new TutorialText("Кристалл!", "Объедини кристаллы, чтобы получить больше."),
                [TutorialStage.Step6CollectCrystalls] = new TutorialText("Больше кристаллов!", "Собери их!"),
                [TutorialStage.Step7_1GetFirstTeaLeaf] = new TutorialText("Новый заказ!", "Нужен чайный пакетик. Тебе потребуется 2 чайных листа."),
                [TutorialStage.Step7_2GetSecondTeaLeaf] = new TutorialText("Новый заказ!", "Нужен чайный пакетик. Тебе потребуется 2 чайных листа."),
                [TutorialStage.Step8MergeTeaLeafs] = new TutorialText("Объедини!", "Ты знаешь, что делать."),
                [TutorialStage.Step9ExecuteSecondOrder] = new TutorialText("Отлично!", "Объединяй предметы, чтобы открывать новые! А сейчас заверши заказ."),
                [TutorialStage.Step10ThrowTrash] = new TutorialText("Фу!", "Клиент оставил мусор. Выброси его!"),
                [TutorialStage.Step11GoToDecoreMode] = new TutorialText("Время покупок!", "Давай обустроим кафе."),
                [TutorialStage.Step12RemoveBoxes] = new TutorialText("Какой бардак!", "Убери коробки."),
                [TutorialStage.Step13StartDialog] = new TutorialText("Поговори!", "Послушай разговор персонажей."),
                [TutorialStage.Step14ReturnToMainScreen] = new TutorialText("Клиенты заждались!", "Давай вернёмся обратно к заказам."),
                [TutorialStage.Step15End] = new TutorialText("Вот и всё!", "Выполняй заказы, зарабатывай кристаллы, обустраивай кафе и пробуй ОБЪЕДИНЯТЬ всё возможное!"),
                [TutorialStage.StepKey1MoveKeyToLock] = new TutorialText("Ключ к счастью!", "Открой дополнительную ячейку."),
                [TutorialStage.StepKey2End] = new TutorialText("Отлично!", "Теперь у тебя больше свободного пространства!"),
                [TutorialStage.StepPresent1OpenPresent] = new TutorialText("Подарок!", "В нём крутые штуки. Открой же его!"),
                [TutorialStage.StepPresent2GetContains] = new TutorialText("Что внутри?", "Посмотри, что там!"),
                [TutorialStage.StepSwitch1] = new TutorialText("Выключение", "По мере расширения оборудования может появиться необходимость временно отключать ненужное."),
                [TutorialStage.StepUpgrade1UpgradeGenerator] = new TutorialText("Время улучшения!", "А что, если объединить чайники?"),
                [TutorialStage.StepUpgrade2End] = new TutorialText("Новый уровень!", "Теперь чайник работает быстрее!"),
                [TutorialStage.StepEnergy1MoveEnergyToGenerator] = new TutorialText("Энергия!", "Ускорь чайник!"),
                [TutorialStage.StepBox1OpenBox] = new TutorialText("Коробка!", "Она содержит случайный предмет из заказа. Испытаем удачу?"),
                [TutorialStage.StepBox2End] = new TutorialText("Отлично!", "Похоже, это именно то, что нам нужно. А что, если мы объединим коробки? :)"),
            }
        };

        public static event Action<Vector2, bool> PlayClickAnimationCursor;
        public static event Action<Vector2, Vector2> PlayDragAnimationCursor;
        public static event Action StopAnimationCursor;
        public static event Action<float> RotateCursor;

        public static event Func<ItemType, Upgradable> GetGenerator;
        public static event Func<int, Transform> GetOrderTransform;
        public static event Func<int, ItemInSequence> GetItemInSequence;
        public static event Func<Transform> GetTrashCanTransform;
        public static event Func<Transform> GetDecoreModeButtonTransform;
        public static event Func<Transform> GetPurchaseButtonTransform;
        public static event Action<int, ItemStorage[], int, int, ItemStorage> GenerateOrder;
        public static event Action<bool> SetActiveGeneratorTimers;
        public static event Action<bool> SetActiveDecoreModeButton;
        public static event Action<ItemType, int> SetGeneratorLevel;
        public static event Action TutorialStarted;
        public static event Action TutorialEnded;

        public static event Action<Vector2, string, string> ShowTutorialWindow;
        public static event Action<string, string> UpdateTutorialWindow;
        public static event Action HideTutorialWindow;
        public static event Action<float> HideTutorialWindowWithDelay;

        public void Save()
        {
            PlayerPrefs.SetInt(TUTORIAL_DONE_KEY, TutorialDone ? 1 : 0);
            for (var i = 0; i < _tutorialItems.Count; ++i)
            {
                PlayerPrefs.SetInt(TUTORIAL_ITEM_TYPE_KEY + i, (int)_tutorialItems[i].Item1);
                PlayerPrefs.SetInt(TUTORIAL_ITEM_LEVEL_KEY + i, _tutorialItems[i].Item2);
            }
            PlayerPrefs.SetInt(TUTORIAL_ITEMS_COUNT_KEY, _tutorialItems.Count);
        }

        public void Load()
        {
            TutorialDone = PlayerPrefs.GetInt(TUTORIAL_DONE_KEY, 0) == 1;
            var count = PlayerPrefs.GetInt(TUTORIAL_ITEMS_COUNT_KEY, 0);
            for (var i = 0; i < count; ++i)
            {
                var item = (ItemType)PlayerPrefs.GetInt(TUTORIAL_ITEM_TYPE_KEY + i);
                var level = PlayerPrefs.GetInt(TUTORIAL_ITEM_LEVEL_KEY + i);
                _tutorialItems.Add((item, level));
            }
        }

        public void StartTutorialMode()
        {
            TutorialDone = false;
            TutorialStarted?.Invoke();
            SetActiveGeneratorTimers?.Invoke(false);
        }

        public void EndTutorialMode()
        {
            TutorialDone = true;
            TutorialEnded?.Invoke();
            SetActiveGeneratorTimers?.Invoke(true);
            StopAnimationCursor?.Invoke();
            //_currentStage = TutorialStage.None;
        }

        public void EndTutorialModeAndHideWindow()
        {
            EndTutorialMode();
            HideTutorialWindow?.Invoke();
        }

        public void CheckTutorialItem(ItemStorage item)
        {
            var key = (item.Type, item.Level);
            if (!TutorialDone || !_tutorialItems.Contains(key))
                return;
            StartTutorialMode();
            _conditionsForTutorial[key]?.Invoke();
            _tutorialItems.Remove(key);
        }

        public void UpdateTutorialText()
        {
            var language = GameStorage.Instance.Language;
            if (!_tutorialTexts.ContainsKey(language) || !_tutorialTexts[language].ContainsKey(_currentStage))
                return;
            var texts = _tutorialTexts[language][_currentStage];
            UpdateTutorialWindow?.Invoke(texts.Title, texts.Text);
        }

        public void OnHideTutorialWindowAfterDelay()
        {
            _currentStage = TutorialStage.None;
        }

        public void OnGeneratorClicked()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step1ClickTeapot:
                    Step2ContinueClickTeapot();
                    break;
                case TutorialStage.Step7_1GetFirstTeaLeaf:
                    StopAnimationCursor?.Invoke();
                    break;
            }
        }

        public void OnItemProduced()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step2ContinueClickTeapot:
                    StartCoroutine(Step3ExecuteFirstOrder());
                    break;
                case TutorialStage.Step7_1GetFirstTeaLeaf:
                    _currentStage = TutorialStage.Step7_2GetSecondTeaLeaf;
                    break;
                case TutorialStage.Step7_2GetSecondTeaLeaf:
                    StartCoroutine(Step8MergeTeaLeafs());
                    break;
            }
        }

        public void OnItemCaptured()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step3ExecuteFirstOrder:
                case TutorialStage.Step5MergeCrystalls:
                case TutorialStage.Step9ExecuteSecondOrder:
                case TutorialStage.Step10ThrowTrash:
                case TutorialStage.StepKey1MoveKeyToLock:
                case TutorialStage.StepUpgrade1UpgradeGenerator:
                    StopAnimationCursor?.Invoke();
                    break;
            }
        }

        public void OnOrderCompleted(int arg)
        {
            switch (_currentStage)
            {
                case TutorialStage.Step3ExecuteFirstOrder:
                    StartCoroutine(Step4GetSequenceReward());
                    break;
                case TutorialStage.Step9ExecuteSecondOrder:
                    Step10ThrowTrash();
                    break;
            }
        }

        public void OnSequencePresentGetted()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step4GetSequenceReward:
                    StartCoroutine(Step5MergeCrystalls());
                    break;
            }
        }

        public void OnMerged()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step5MergeCrystalls:
                    Step6CollectCrystalls();
                    break;
                case TutorialStage.Step8MergeTeaLeafs:
                    Step9ExecuteSecondOrder();
                    break;
            }
        }

        public void OnCurrencyCollected()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step6CollectCrystalls:
                    Step7GetTwoTeaLeaf();
                    break;
            }
        }

        public void OnThrowed()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step10ThrowTrash:
                    Step11GoToDecoreMode();
                    break;
            }
        }

        public void OnClickedDecorateButton()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step11GoToDecoreMode:
                    StartCoroutine(Step12RemoveBoxes());
                    break;
                case TutorialStage.Step14ReturnToMainScreen:
                    Step15End();
                    break;
            }
        }

        public void OnPurchased()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step12RemoveBoxes:
                    StartCoroutine(Step13StartDialog());
                    break;
                case TutorialStage.Step13StartDialog:
                    HideTutorialWindow?.Invoke();
                    break;
            }
        }

        public void OnDialogEnded()
        {
            switch (_currentStage)
            {
                case TutorialStage.Step13StartDialog:
                    Step14ReturnToMainScreen();
                    break;
            }
        }

        public void OnItemsCombinated()
        {
            switch (_currentStage)
            {
                case TutorialStage.StepKey1MoveKeyToLock:
                    StepKey2End();
                    break;
            }
        }

        public void OnGeneratorUpgraded()
        {
            switch (_currentStage)
            {
                case TutorialStage.StepUpgrade1UpgradeGenerator:
                    StepUpgrade2End();
                    break;
            }
        }

        public void OnGeneratorSpeeded()
        {
            switch (_currentStage)
            {
                case TutorialStage.StepEnergy1MoveEnergyToGenerator:
                    EndTutorialModeAndHideWindow();
                    break;
            }
        }

        public void OnOpenedPresent()
        {
            switch (_currentStage)
            {
                case TutorialStage.StepPresent1OpenPresent:
                    StepPresent2GetContains();
                    break;
            }
        }

        public void OnClickedOpenPresent()
        {
            switch (_currentStage)
            {
                case TutorialStage.StepPresent2GetContains:
                    EndTutorialModeAndHideWindow();
                    break;
            }
        }

        public void OnOpenedBox()
        {
            switch (_currentStage)
            {
                case TutorialStage.StepBox1OpenBox:
                    StepBox2End();
                    break;
            }
        }

        public void OnNewGeneratorAppears(ItemType type)
        {
            if (type is ItemType.Oven && PlayerPrefs.GetInt(TUTORIAL_GENERATOR_SWITCH_COMPLETED_KEY, 0) == 0)
                StartCoroutine(StepSwitch1());
        }

        public void OnGeneratorSwitched(bool arg)
        {
            switch (_currentStage)
            {
                case TutorialStage.StepSwitch1:
                    EndTutorialModeAndHideWindow();
                    break;
            }
        }

        private void Awake()
        {
            Load();

            _conditionsForTutorial = new Dictionary<(ItemType, int), Action>
            {
                [(ItemType.Key, 1)] = () => StartCoroutine(StepKey1MoveKeyToLock()),
                [(ItemType.Teapot, 1)] = () => StartCoroutine(StepUpgrade1UpgradeGenerator()),
                [(ItemType.Energy, 2)] = () => StartCoroutine(StepEnergy1MoveEnergyToGenerator()),
                [(ItemType.Present, 1)] = () => StartCoroutine(StepPresent1OpenPresent()),
                [(ItemType.Box, 2)] = () => StartCoroutine(StepBox1OpenBox()),
            };
        }

        private void Start()
        {
            _storage = GameStorage.Instance;

            if (!TutorialDone)
            {
                _tutorialItems = _conditionsForTutorial.Keys.ToList();
                if (_skipStartTutorial)
                {
                    TutorialDone = true;
                    GenerateOrder?.Invoke(0, new[] { GameStorage.Instance.GetItem(ItemType.Tea, 1) }, 1, 10, null);
                }
                else
                {
                    PlayerPrefs.DeleteAll();
                    StartTutorialMode();
                    Step1ClickTeapot();
                }
            }
        }

        private void Step1ClickTeapot()
        {
            _currentStage = TutorialStage.Step1ClickTeapot;
            CanRandomOrders = false;
            SetGeneratorLevel?.Invoke(ItemType.Teapot, 7);
            GenerateOrder?.Invoke(0, new[] { _storage.GetItem(ItemType.Tea, 1) }, 1, 5, _storage.GetItem(ItemType.Brilliant, 1));
            SetNewTutorialTarget(GetGenerator(ItemType.Teapot).gameObject);
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step2ContinueClickTeapot()
        {
            _currentStage = TutorialStage.Step2ContinueClickTeapot;
            StopAnimationCursor?.Invoke();
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator Step3ExecuteFirstOrder()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.Step3ExecuteFirstOrder;
            SetNewTutorialTarget(_storage.GetItemsOnField(ItemType.Tea, 1, 1)[0].gameObject);
            PlayDragAnimationCursor?.Invoke(_target.transform.position, GetOrderTransform(0).position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator Step4GetSequenceReward()
        {
            yield return new WaitForSeconds(0f);
            _currentStage = TutorialStage.Step4GetSequenceReward;
            SoundManager.Instanse.Play(Sound.Achivement, null);
            GenerateOrder?.Invoke(0, new[] { _storage.GetItem(ItemType.Tea, 2) }, 2, 10, _storage.GetItem(ItemType.Trash, 1));
            var itemInSequence = GetItemInSequence(1);
            SetNewTutorialTarget(itemInSequence.gameObject);
            RotateCursor?.Invoke(-120f);
            PlayClickAnimationCursor?.Invoke(itemInSequence.GetRewardImage().transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator Step5MergeCrystalls()
        {
            StopAnimationCursor?.Invoke();
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.Step5MergeCrystalls;
            var crystalls = _storage.GetItemsOnField(ItemType.Brilliant, 1, 2);
            SetNewTutorialTarget(crystalls[0].gameObject);
            RotateCursor?.Invoke(0f);
            PlayDragAnimationCursor?.Invoke(crystalls[0].transform.position, crystalls[1].transform.position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step6CollectCrystalls()
        {
            _currentStage = TutorialStage.Step6CollectCrystalls;
            _storage.GetItemsOnField(ItemType.Brilliant, 2, 1)[0].gameObject.AddComponent<TutorialExtraTarget>();
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step7GetTwoTeaLeaf()
        {
            _currentStage = TutorialStage.Step7_1GetFirstTeaLeaf;
            SetNewTutorialTarget(GetGenerator(ItemType.Teapot).gameObject);
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator Step8MergeTeaLeafs()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.Step8MergeTeaLeafs;
            var teaLeafs = _storage.GetItemsOnField(ItemType.Tea, 1, 2);
            SetNewTutorialTarget(teaLeafs[0].gameObject);
            PlayDragAnimationCursor(teaLeafs[0].transform.position, teaLeafs[1].transform.position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step9ExecuteSecondOrder()
        {
            _currentStage = TutorialStage.Step9ExecuteSecondOrder;
            SetNewTutorialTarget(_storage.GetItemsOnField(ItemType.Tea, 2, 1)[0].gameObject);
            PlayDragAnimationCursor?.Invoke(_target.transform.position, GetOrderTransform(0).position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step10ThrowTrash()
        {
            _currentStage = TutorialStage.Step10ThrowTrash;
            CanRandomOrders = true;
            SetGeneratorLevel?.Invoke(ItemType.Teapot, 1);
            SetNewTutorialTarget(_storage.GetItemsOnField(ItemType.Trash, 1, 1)[0].gameObject);
            _target.AddComponent<TutorialExtraTarget>();
            PlayDragAnimationCursor(_target.transform.position, GetTrashCanTransform().position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step11GoToDecoreMode()
        {
            _currentStage = TutorialStage.Step11GoToDecoreMode;
            RemoveTarget();
            SetActiveDecoreModeButton?.Invoke(true);
            RotateCursor?.Invoke(90f);
            PlayClickAnimationCursor(GetDecoreModeButtonTransform().position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator Step12RemoveBoxes()
        {
            yield return new WaitForEndOfFrame();
            _currentStage = TutorialStage.Step12RemoveBoxes;
            SetActiveDecoreModeButton?.Invoke(false);
            SetNewTutorialTarget(GetPurchaseButtonTransform().gameObject);
            StopAnimationCursor?.Invoke();
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator Step13StartDialog()
        {
            yield return new WaitForEndOfFrame();
            _currentStage = TutorialStage.Step13StartDialog;
            SetNewTutorialTarget(GetPurchaseButtonTransform().gameObject);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step14ReturnToMainScreen()
        {
            _currentStage = TutorialStage.Step14ReturnToMainScreen;
            SetActiveDecoreModeButton?.Invoke(true);
            PlayClickAnimationCursor(GetDecoreModeButtonTransform().position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void Step15End()
        {
            _currentStage = TutorialStage.Step15End;
            RotateCursor?.Invoke(0f);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
            HideTutorialWindowWithDelay?.Invoke(10f);
            EndTutorialMode();
        }

        private IEnumerator StepKey1MoveKeyToLock()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.StepKey1MoveKeyToLock;
            var keys = GameStorage.Instance.GetItemsOnField(ItemType.Key, 1, 1);
            var cellLocks = GameStorage.Instance.GetItemsOnField(ItemType.Lock, 1, 1);
            if (keys.Length == 0 || cellLocks.Length == 0)
            {
                EndTutorialMode();
                yield break;
            }
            SetNewTutorialTarget(keys[0].gameObject);
            PlayDragAnimationCursor?.Invoke(keys[0].transform.position, cellLocks[0].transform.position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void StepKey2End()
        {
            _currentStage = TutorialStage.StepKey2End;
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
            HideTutorialWindowWithDelay?.Invoke(6f);
            EndTutorialMode();
        }

        private IEnumerator StepUpgrade1UpgradeGenerator()
        {
            yield return new WaitForSeconds(0.5f);
            var teapot = GetGenerator(ItemType.Teapot);
            if (teapot.Level > 1)
            {
                _tutorialItems.Remove((ItemType.Teapot, 1));
                EndTutorialMode();
                yield break;
            }
            _currentStage = TutorialStage.StepUpgrade1UpgradeGenerator;
            var teapotItems = GameStorage.Instance.GetItemsOnField(ItemType.Teapot, 1, 1);
            if (teapotItems.Length == 0)
            {
                EndTutorialMode();
                yield break;
            }
            SetNewTutorialTarget(teapotItems[0].gameObject);
            PlayDragAnimationCursor?.Invoke(teapotItems[0].transform.position, teapot.transform.position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void StepUpgrade2End()
        {
            _currentStage = TutorialStage.StepUpgrade2End;
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
            HideTutorialWindowWithDelay?.Invoke(6f);
            EndTutorialMode();
        }

        private IEnumerator StepEnergy1MoveEnergyToGenerator()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.StepEnergy1MoveEnergyToGenerator;
            var energy = GameStorage.Instance.GetItemsOnField(ItemType.Energy, 2, 1);
            if (energy.Length == 0)
            {
                EndTutorialMode();
                yield break;
            }
            SetNewTutorialTarget(energy[0].gameObject);
            PlayDragAnimationCursor?.Invoke(energy[0].transform.position, GetGenerator(ItemType.Teapot).transform.position);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator StepPresent1OpenPresent()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.StepPresent1OpenPresent;
            var presents = GameStorage.Instance.GetItemsOnField(ItemType.Present, 1, 1);
            if (presents.Length == 0)
            {
                EndTutorialMode();
                yield break;
            }
            SetNewTutorialTarget(presents[0].gameObject);
            PlayClickAnimationCursor?.Invoke(presents[0].transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void StepPresent2GetContains()
        {
            _currentStage = TutorialStage.StepPresent2GetContains;
            var openPresent = GameStorage.Instance.GetItemsOnField(ItemType.OpenPresent, 1, 1)[0];
            SetNewTutorialTarget(openPresent.gameObject);
            PlayClickAnimationCursor?.Invoke(openPresent.transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private IEnumerator StepBox1OpenBox()
        {
            yield return new WaitForSeconds(0.5f);
            if (!GameStorage.Instance.HasEmptyCells())
            {
                _tutorialItems.Add((ItemType.Box, 2));
                EndTutorialMode();
                yield break;
            }
            _currentStage = TutorialStage.StepBox1OpenBox;
            var boxes = GameStorage.Instance.GetItemsOnField(ItemType.Box, 2, 1);
            if (boxes.Length == 0)
            {
                EndTutorialMode();
                yield break;
            }
            SetNewTutorialTarget(boxes[0].gameObject);
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void StepBox2End()
        {
            _currentStage = TutorialStage.StepBox2End;
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
            HideTutorialWindowWithDelay?.Invoke(10f);
            EndTutorialMode();
        }

        private IEnumerator StepSwitch1()
        {
            yield return new WaitForSeconds(2f);
            PlayerPrefs.SetInt(TUTORIAL_GENERATOR_SWITCH_COMPLETED_KEY, 1);
            StartTutorialMode();
            _currentStage = TutorialStage.StepSwitch1;
            var teapotToggle = GetGenerator?.Invoke(ItemType.Teapot).GetComponent<ItemGenerator>().GetToggle();
            teapotToggle.interactable = true;
            teapotToggle.onValueChanged.AddListener(OnGeneratorSwitched);
            PlayClickAnimationCursor?.Invoke(teapotToggle.transform.position, true);
            ShowTutorialWin(_rightTopSpawnWindowPoint.position);
        }

        private void SetNewTutorialTarget(GameObject target)
        {
            RemoveTarget();
            _target = target;
            _target.AddComponent<TutorialTarget>();
        }

        private void RemoveTarget()
        {
            if (_target != null)
                Destroy(_target.GetComponent<TutorialTarget>());
            _target = null;
        }

        private void ShowTutorialWin(Vector2 position)
        {
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "", "");
            UpdateTutorialText();
        }

        public class TutorialText
        {
            public string Title { get; }
            public string Text { get; }

            public TutorialText(string title, string text)
            {
                Title = title;
                Text = text;
            }
        }
    }
}