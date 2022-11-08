using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Enums;

namespace Gameplay.DecorationMode.Dialogs
{
    public class DialogCreator : MonoBehaviour
    {
        [SerializeField] private float delayBetweenChars = 0.1f;
        [SerializeField] private Image _characterLeft;
        [SerializeField] private Image _characterRight;
        [SerializeField] private TextMeshProUGUI _nameLeft;
        [SerializeField] private TextMeshProUGUI _nameRight;
        [SerializeField] private TextMeshProUGUI _phrase;
        [SerializeField] private GameObject _blackFilter;
        [SerializeField] private GameObject _background;
        [SerializeField] private Character[] _characters;
        [SerializeField] private DialogPart[] _dialogCollection;

        private Coroutine _pringPhraseCoroutine;
        private GameObject _purchaseCanvas;

        private readonly Dictionary<CharacterName, Character> _charactersDict = new Dictionary<CharacterName, Character>();
        private readonly Dictionary<DialogTitle, DialogPhrase[]> _dialogDict = new Dictionary<DialogTitle, DialogPhrase[]>();
        private int _currentIndex = 0;
        private DialogTitle _currentDialogTitle;
        private bool _phraseIsPrinting = false;

        public void Begin(DialogTitle title)
        {
            if (_purchaseCanvas == null)
                _purchaseCanvas = GameObject.FindGameObjectWithTag(Tags.BuildingCanvas.ToString());

            _currentIndex = 0;
            _currentDialogTitle = title;
            StartCoroutine(SetActive(true));
            NextPhrase();
        }

        public void Skip() => StartCoroutine(SetActive(false));

        private void Awake()
        {
            foreach (var character in _characters)
            {
                _charactersDict.Add(character.Type, character);
                character.Initialize();
            }

            foreach (var dialogPart in _dialogCollection)
                _dialogDict.Add(dialogPart.Type, dialogPart.Phrases);
        }

        private void Update()
        {
            if (!_background.activeSelf)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (_phraseIsPrinting)
                {
                    if (_pringPhraseCoroutine != null)
                        StopCoroutine(_pringPhraseCoroutine);
                    _phraseIsPrinting = false;
                    _phrase.text = _dialogDict[_currentDialogTitle][_currentIndex - 1].Phrase;
                }
                else
                    NextPhrase();
            }
        }

        private void NextPhrase()
        {
            if (_currentIndex >= _dialogDict[_currentDialogTitle].Length)
            {
                StartCoroutine(SetActive(false));
                return;
            }

            var phrase = _dialogDict[_currentDialogTitle][_currentIndex++];
            SetCharacter(_charactersDict[phrase.Character]);
            _pringPhraseCoroutine = StartCoroutine(PrintPhrase(phrase.Phrase));
        }

        private void SetCharacter(Character character)
        {
            _characterLeft.gameObject.SetActive(character.LeftSide);
            _nameLeft.gameObject.SetActive(character.LeftSide);
            _characterRight.gameObject.SetActive(!character.LeftSide);
            _nameRight.gameObject.SetActive(!character.LeftSide);

            if (character.LeftSide)
            {
                SetFace(_characterLeft);
                SetName(_nameLeft);
            }
            else
            {
                SetFace(_characterRight);
                SetName(_nameRight);
            }

            void SetFace(Image toImage) => toImage.sprite = character.GetEmotion(_dialogDict[_currentDialogTitle][_currentIndex - 1].Emotion);
            void SetName(TextMeshProUGUI toText) => toText.text = character.Name;
        }

        private IEnumerator PrintPhrase(string phrase)
        {
            _phrase.text = "";
            _phraseIsPrinting = true;
            foreach (var c in phrase)
            {
                _phrase.text += c;
                yield return new WaitForSeconds(delayBetweenChars);
            }
            _phraseIsPrinting = false;
        }

        private IEnumerator SetActive(bool value)
        {
            yield return new WaitForSeconds(0.01f);
            _background.SetActive(value);
            _blackFilter.SetActive(value);
            _purchaseCanvas.SetActive(!value);
        }
    }
}