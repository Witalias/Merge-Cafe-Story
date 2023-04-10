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
            _currentStage = TutorialStage.None;
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

        private void Step1ClickTeapot()
        {
            _currentStage = TutorialStage.Step1ClickTeapot;
            CanRandomOrders = false;
            SetGeneratorLevel?.Invoke(ItemType.Teapot, 7);
            GenerateOrder?.Invoke(0, new[] { _storage.GetItem(ItemType.Tea, 1) }, 1, 5, _storage.GetItem(ItemType.Brilliant, 1));
            SetNewTutorialTarget(GetGenerator(ItemType.Teapot).gameObject);
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Let's get started!", "Make some tea.");
        }

        private void Step2ContinueClickTeapot()
        {
            _currentStage = TutorialStage.Step2ContinueClickTeapot;
            StopAnimationCursor?.Invoke();
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Great!", "Keep clicking until the bar fills up.");
        }

        private IEnumerator Step3ExecuteFirstOrder()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.Step3ExecuteFirstOrder;
            SetNewTutorialTarget(_storage.GetItemsOnField(ItemType.Tea, 1, 1)[0].gameObject);
            PlayDragAnimationCursor?.Invoke(_target.transform.position, GetOrderTransform(0).position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Complete the order!", "Drag the tea leaf to order.");
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
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "What's shining there?", "You have completed the first order! Get a reward!");
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
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Crystal!", "Merge crystals to get more!");
        }

        private void Step6CollectCrystalls()
        {
            _currentStage = TutorialStage.Step6CollectCrystalls;
            _storage.GetItemsOnField(ItemType.Brilliant, 2, 1)[0].gameObject.AddComponent<TutorialExtraTarget>();
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "More crystals!", "Collect them!");
        }

        private void Step7GetTwoTeaLeaf()
        {
            _currentStage = TutorialStage.Step7_1GetFirstTeaLeaf;
            SetNewTutorialTarget(GetGenerator(ItemType.Teapot).gameObject);
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "New order!", "You need a tea bag. First let's get 2 tea leaves.");
        }

        private IEnumerator Step8MergeTeaLeafs()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.Step8MergeTeaLeafs;
            var teaLeafs = _storage.GetItemsOnField(ItemType.Tea, 1, 2);
            SetNewTutorialTarget(teaLeafs[0].gameObject);
            PlayDragAnimationCursor(teaLeafs[0].transform.position, teaLeafs[1].transform.position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Merge!", "You know what to do.");
        }

        private void Step9ExecuteSecondOrder()
        {
            _currentStage = TutorialStage.Step9ExecuteSecondOrder;
            SetNewTutorialTarget(_storage.GetItemsOnField(ItemType.Tea, 2, 1)[0].gameObject);
            PlayDragAnimationCursor?.Invoke(_target.transform.position, GetOrderTransform(0).position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Done!", "Merge items to unlock new ones! Now complete the order.");
        }

        private void Step10ThrowTrash()
        {
            _currentStage = TutorialStage.Step10ThrowTrash;
            CanRandomOrders = true;
            SetGeneratorLevel?.Invoke(ItemType.Teapot, 1);
            SetNewTutorialTarget(_storage.GetItemsOnField(ItemType.Trash, 1, 1)[0].gameObject);
            _target.AddComponent<TutorialExtraTarget>();
            PlayDragAnimationCursor(_target.transform.position, GetTrashCanTransform().position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Ugh!", "The customer left the garbage. Throw it away!");
        }

        private void Step11GoToDecoreMode()
        {
            _currentStage = TutorialStage.Step11GoToDecoreMode;
            RemoveTarget();
            SetActiveDecoreModeButton?.Invoke(true);
            RotateCursor?.Invoke(90f);
            PlayClickAnimationCursor(GetDecoreModeButtonTransform().position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Shopping time!", "Let's decorate the cafe.");
        }

        private IEnumerator Step12RemoveBoxes()
        {
            yield return new WaitForEndOfFrame();
            _currentStage = TutorialStage.Step12RemoveBoxes;
            SetActiveDecoreModeButton?.Invoke(false);
            SetNewTutorialTarget(GetPurchaseButtonTransform().gameObject);
            StopAnimationCursor?.Invoke();
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "What a mess!", "Put the boxes away.");
        }

        private IEnumerator Step13StartDialog()
        {
            yield return new WaitForEndOfFrame();
            _currentStage = TutorialStage.Step13StartDialog;
            SetNewTutorialTarget(GetPurchaseButtonTransform().gameObject);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Talk!", "Listen to what the characters are talking about.");
        }

        private void Step14ReturnToMainScreen()
        {
            _currentStage = TutorialStage.Step14ReturnToMainScreen;
            SetActiveDecoreModeButton?.Invoke(true);
            PlayClickAnimationCursor(GetDecoreModeButtonTransform().position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Customers are waiting!", "Let's go back to the orders.");
        }

        private void Step15End()
        {
            _currentStage = TutorialStage.Step15End;
            EndTutorialMode();
            RotateCursor?.Invoke(0f);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "That's it!", "Fulfill orders, receive crystals, equip the cafe and try to MERGE EVERYTHING possible!");
            HideTutorialWindowWithDelay?.Invoke(10f);
        }

        private IEnumerator StepKey1MoveKeyToLock()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.StepKey1MoveKeyToLock;
            var keys = GameStorage.Instance.GetItemsOnField(ItemType.Key, 1, 1);
            var cellLocks = GameStorage.Instance.GetItemsOnField(ItemType.Lock, 1, 1);
            if (keys.Length == 0 || cellLocks.Length == 0)
                yield break;
            SetNewTutorialTarget(keys[0].gameObject);
            PlayDragAnimationCursor?.Invoke(keys[0].transform.position, cellLocks[0].transform.position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "The key to happiness!", "Unlock an additional cell.");
        }

        private void StepKey2End()
        {
            _currentStage = TutorialStage.StepKey2End;
            EndTutorialMode();
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Great!", "Now you have more free space!");
            HideTutorialWindowWithDelay?.Invoke(6f);
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
                yield break;
            SetNewTutorialTarget(teapotItems[0].gameObject);
            PlayDragAnimationCursor?.Invoke(teapotItems[0].transform.position, teapot.transform.position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Upgrade time!", "How about merging the kettle?");
        }

        private void StepUpgrade2End()
        {
            _currentStage = TutorialStage.StepUpgrade2End;
            EndTutorialMode();
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Level up!", "Now the kettle works faster!");
            HideTutorialWindowWithDelay?.Invoke(6f);
        }

        private IEnumerator StepEnergy1MoveEnergyToGenerator()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.StepEnergy1MoveEnergyToGenerator;
            var energy = GameStorage.Instance.GetItemsOnField(ItemType.Energy, 2, 1);
            if (energy.Length == 0)
                yield break;
            SetNewTutorialTarget(energy[0].gameObject);
            PlayDragAnimationCursor?.Invoke(energy[0].transform.position, GetGenerator(ItemType.Teapot).transform.position);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Energy!", "Speed up the kettle!");
        }

        private IEnumerator StepPresent1OpenPresent()
        {
            yield return new WaitForSeconds(0.5f);
            _currentStage = TutorialStage.StepPresent1OpenPresent;
            var presents = GameStorage.Instance.GetItemsOnField(ItemType.Present, 1, 1);
            if (presents.Length == 0)
                yield break;
            SetNewTutorialTarget(presents[0].gameObject);
            PlayClickAnimationCursor?.Invoke(presents[0].transform.position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Present!", "It contains useful items. Open it!");
        }

        private void StepPresent2GetContains()
        {
            _currentStage = TutorialStage.StepPresent2GetContains;
            var openPresent = GameStorage.Instance.GetItemsOnField(ItemType.OpenPresent, 1, 1)[0];
            SetNewTutorialTarget(openPresent.gameObject);
            PlayClickAnimationCursor?.Invoke(openPresent.transform.position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "What's inside?", "Look what's in there!");
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
                yield break;
            SetNewTutorialTarget(boxes[0].gameObject);
            PlayClickAnimationCursor?.Invoke(_target.transform.position, true);
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Box!", "It contains an item from the order. Try our luck?");
        }

        private void StepBox2End()
        {
            _currentStage = TutorialStage.StepBox2End;
            EndTutorialMode();
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Great!", "It looks like this is what we need. And what happens if we merge boxes? :)");
            HideTutorialWindowWithDelay?.Invoke(10f);
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
            ShowTutorialWindow?.Invoke(_rightTopSpawnWindowPoint.position, "Shutdown", "As the equipment expands, you may need to temporarily disable unnecessary ones.");
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
    }
}