﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;
using DG.Tweening;
using EA4S;
using EA4S.Db;

namespace EA4S
{
    public class WidgetPopupWindow : MonoBehaviour
    {
        public static WidgetPopupWindow I;

        public static bool IsShown { get; private set; }

        [Header("Options")]
        public bool timeIndependent = true;
        [Header("References")]
        public GameObject Window;
        public GameObject TitleGO;
        public GameObject TitleEnglishGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject MessageTextGO;
        public UIButton ButtonGO;
        public GameObject TutorialImageGO;
        public GameObject MarkOK;
        public GameObject MarkKO;
        public Sprite gameTimeUpSprite;

        bool clicked;
        Action currentCallback;
        Tween showTween;

        void Awake()
        {
            I = this;

            showTween = this.GetComponent<RectTransform>().DOAnchorPosY(-800, 0.5f).From().SetUpdate(timeIndependent)
                .SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(() => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        public void ResetContents()
        {
            clicked = false;
            TutorialImageGO.SetActive(false);
            SetTitle("", false);
            SetWord("", "");
            MarkOK.SetActive(false);
            MarkKO.SetActive(false);
            SetMessage("", false);
        }

        public void Close(bool _immediate = false)
        {
            if (IsShown || _immediate)
                Show(false, _immediate);
        }

        public void Show(bool _doShow, bool _immediate = false)
        {
            GlobalUI.Init();

            IsShown = _doShow;
            if (_doShow) {
                clicked = false;
                if (_immediate)
                    I.showTween.Complete();
                else
                    I.showTween.PlayForward();
            } else {
                if (_immediate)
                    I.showTween.Rewind();
                else
                    I.showTween.PlayBackwards();
            }
        }


        public void ShowTextDirect(Action callback, string myText)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            TitleGO.GetComponent<TextMeshProUGUI>().text = myText;
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = "";

            Show(true);
        }

        public void SetButtonCallback(Action callback)
        {
            currentCallback = callback;
        }

        public void ShowArabicTextDirect(Action callback, string myArabicText)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(myArabicText, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = "";

            Show(true);
        }

        public void ShowSentence(Action callback, string SentenceId)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            Db.LocalizationData row = LocalizationManager.GetLocalizationData(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.Arabic, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.English;

            AudioManager.I.PlayDialog(SentenceId);

            Show(true);
        }

        public void ShowSentence(Action callback, string sentenceId, Sprite image2show)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            if (image2show != null) {
                TutorialImageGO.GetComponent<Image>().sprite = image2show;
                TutorialImageGO.SetActive(true);
            }

            Db.LocalizationData row = LocalizationManager.GetLocalizationData(sentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.Arabic, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.English;

            AudioManager.I.PlayDialog(sentenceId);

            Show(true);
        }

        public void ShowSentenceWithMark(Action callback, string sentenceId, bool result, Sprite image2show)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            MarkOK.SetActive(result);
            MarkKO.SetActive(!result);

            if (image2show != null) {
                TutorialImageGO.GetComponent<Image>().sprite = image2show;
                TutorialImageGO.SetActive(true);
            }

            Db.LocalizationData row = LocalizationManager.GetLocalizationData(sentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.Arabic, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.English;

            AudioManager.I.PlayDialog(sentenceId);

            Show(true);
        }

        public void SetMark(bool visible, bool ok)
        {
            MarkOK.SetActive(ok);
            MarkKO.SetActive(!ok);
        }

        public void SetImage(Sprite image2show)
        {
            if (image2show != null)
            {
                TutorialImageGO.GetComponent<Image>().sprite = image2show;
                TutorialImageGO.SetActive(true);
            }
            else
                TutorialImageGO.SetActive(false);
        }


        public void ShowStringAndWord(Action callback, string text, LL_WordData wordData)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            TitleGO.GetComponent<TextMeshProUGUI>().text = text;
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = "";

            //AudioManager.I.PlayDialog(SentenceId);

            SetWord(wordData.Key, wordData.Data.Arabic);

            Show(true);
        }

        public void ShowSentenceAndWord(Action callback, string SentenceId, LL_WordData wordData)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            Db.LocalizationData row = LocalizationManager.GetLocalizationData(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.Arabic, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.English;

            //AudioManager.I.PlayDialog(SentenceId);

            SetWord(wordData.Key, wordData.Data.Arabic);

            Show(true);
        }

        public void ShowSentenceAndWordWithMark(Action callback, string SentenceId, LL_WordData wordData, bool result)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);

            MarkOK.SetActive(result);
            MarkKO.SetActive(!result);

            Db.LocalizationData row = LocalizationManager.GetLocalizationData(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.Arabic, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.English;

            //AudioManager.I.PlayDialog(SentenceId);

            SetWord(wordData.Key, wordData.Data.Arabic);

            Show(true);
        }

        public void ShowTimeUp(Action callback)
        {
            ShowSentence(callback, TextID.TIMES_UP.ToString(), gameTimeUpSprite);
        }

        public void Init(string introText, string wordCode, string arabicWord)
        {
            Init(null, introText, wordCode, arabicWord);
        }

        public void ShowTutorial(Action callback, Sprite tutorialImage)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);
            TutorialImageGO.GetComponent<Image>().sprite = tutorialImage;
            TutorialImageGO.SetActive(true);

            AudioManager.I.PlaySfx(Sfx.UIPopup);
            Show(true);
        }

        public void Init(Action callback, string introText, string wordCode, string arabicWord)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.gameObject.SetActive(callback != null);
            TutorialImageGO.SetActive(false);

            SetTitle(introText, false);
            SetWord(wordCode, arabicWord);
            //            Window.SetActive(true);
        }

        public void SetTitle(string text, bool isArabic)
        {
            if (isArabic)
                TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
            else
                TitleGO.GetComponent<TextMeshProUGUI>().text = text;

            /*
            if (isArabic)
            {
                TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
                TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                TitleGO.GetComponent<TextMeshProUGUI>().text = "";
                TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = text;
            }
            */
        }

        public void SetMessage(string text, bool isArabic)
        {
            if (isArabic)
            {
                MessageTextGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
            }
            else
            {
                MessageTextGO.GetComponent<TextMeshProUGUI>().text = text;
            }
        }

        public void SetTitleSentence(string SentenceId)
        {
            Db.LocalizationData row = LocalizationManager.GetLocalizationData(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.Arabic, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.English;
        }

        public void SetWord(string wordCode, string arabicWord)
        {
            if (wordCode != "") {
                WordTextGO.SetActive(true);
                DrawingImageGO.SetActive(true);
                // here set both word and drawing 
                WordTextGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(arabicWord, false, false);
                DrawingImageGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);
            } else {
                WordTextGO.SetActive(false);
                DrawingImageGO.SetActive(false);
            }
        }

        public void OnPressButtonPanel()
        {
            //Debug.Log("OnPressButtonPanel() " + clicked);
            OnPressButton();
        }

        public void OnPressButton()
        {
            //Debug.Log("OnPressButton() " + clicked);
            if (clicked)
                return;

            clicked = true;
            ButtonGO.AnimateClick();
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);

            if (currentCallback != null)
                currentCallback();
        }
    }
}