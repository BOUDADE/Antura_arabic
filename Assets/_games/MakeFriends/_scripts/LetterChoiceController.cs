﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using TMPro;

namespace EA4S.MakeFriends
{
    public class LetterChoiceController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public TMP_Text LetterText;
        public Animator animator;
        public Image image;
        public Button button;
        public CanvasGroup canvasGroup;

        [HideInInspector]
        public LL_LetterData letterData;
        [HideInInspector]
        public bool wasChosen;

        public enum ChoiceState
        {
            IDLE,
            CORRECT,
            WRONG
        }

        public ChoiceState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged();
                }
            }
        }

        private ChoiceState _state;
        private bool disabled;
        private Vector2 initialPosition = Vector2.zero;


        public void Init(LL_LetterData _letterData)
        {
            Reset();
            letterData = _letterData;
            LetterText.text = ArabicAlphabetHelper.GetLetterFromUnicode(letterData.Data.Isolated_Unicode);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (disabled)
            {
                return;
            }

            //Disable();
            SpeakLetter();
            //MakeFriendsGameManager.Instance.OnClickedLetterChoice(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (disabled)
            {
                return;
            }

            initialPosition = transform.position;
            MakeFriendsGameManager.Instance.letterPicker.letterChoiceBeingDragged = this;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (disabled)
            {
                return;
            }

            transform.position = eventData.position;

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (disabled)
            {
                return;
            }
            
            if (wasChosen)
            {
                Disable();
                MakeFriendsGameManager.Instance.OnLetterChoiceSelected(this);
            }
            else
            {
                transform.position = initialPosition;
            }

            MakeFriendsGameManager.Instance.letterPicker.letterChoiceBeingDragged = null;
            canvasGroup.blocksRaycasts = true;
        }

        public void SpeakLetter()
        {
            if (letterData != null && letterData.Key != null)
            {
                AudioManager.I.PlayLetter(letterData.Key);
            }
        }

        public void FlashWrong()
        {
            animator.SetTrigger("FlashWrong");
        }

        public void SpawnBalloon(bool correctChoice)
        {
            var balloon = Instantiate(MakeFriendsGameManager.Instance.letterBalloonPrefab, MakeFriendsGameManager.Instance.letterBalloonContainer.transform.position, Quaternion.identity, MakeFriendsGameManager.Instance.letterBalloonContainer.transform) as GameObject;
            var balloonController = balloon.GetComponent<LetterBalloonController>();
            balloonController.Init(letterData);
            balloonController.EnterScene(correctChoice);
        }

        private void Disable()
        {
            disabled = true;
            image.enabled = false;
            button.enabled = false;
            LetterText.enabled = false;
        }

        private void Reset()
        {
            disabled = false;
            wasChosen = false;
            image.enabled = true;
            button.enabled = true;
            LetterText.enabled = true;
            State = ChoiceState.IDLE;
        }

        private void OnStateChanged()
        {
            switch (State)
            {
                case ChoiceState.IDLE:
                    animator.SetTrigger("Idle");
                    break;
                case ChoiceState.CORRECT:
                    animator.SetTrigger("Correct");
                    break;
                case ChoiceState.WRONG:
                    animator.SetTrigger("Wrong");
                    break;
                default:
                    break;
            }
        }
    }
}