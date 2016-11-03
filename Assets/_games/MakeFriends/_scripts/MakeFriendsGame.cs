﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;

namespace EA4S.MakeFriends
{
    public class MakeFriendsGame : MiniGame
    {
        public LivingLetterArea leftArea;
        public LivingLetterArea rightArea;
        public LetterPickerController letterPicker;
        public Canvas endGameCanvas;
        public StarFlowers starFlowers;
        public GameObject sceneCamera;
        public static int numberOfRounds = 6;
        public float uiDelay;
        public Vector3 endCameraPosition;
        public Vector3 endCameraRotation;
        public GameObject letterBalloonPrefab;
        public GameObject letterBalloonContainer;
        public GameObject FxParticlesPoof;
        public DropZoneController dropZone;
        [Header("Difficulty Override")]
        public bool overrideDifficulty;
        public MakeFriendsVariation difficultySetting;
        new public static MakeFriendsGame Instance;
        [Header("Gameplay Info")]
        new public MakeFriendsGameplayInfo GameplayInfo;

        [HideInInspector]
        public MakeFriendsConfiguration Configuration { get { return MakeFriendsConfiguration.Instance; } }

        private LL_WordData wordData1;
        private List<LL_LetterData> wordLetters1 = new List<LL_LetterData>();
        private LL_WordData wordData2;
        private List<LL_LetterData> wordLetters2 = new List<LL_LetterData>();
        private List<LL_LetterData> commonLetters = new List<LL_LetterData>();
        private List<LL_LetterData> uncommonLetters = new List<LL_LetterData>();
        private List<LL_LetterData> choiceLetters = new List<LL_LetterData>();
        private List<LL_LetterData> correctChoices = new List<LL_LetterData>();
        private List<LL_LetterData> incorrectChoices = new List<LL_LetterData>();
        private int currentRound = 0;
        private int friendships = 0;

        public MakeFriendsIntroductionState IntroductionState { get; private set; }

        public MakeFriendsQuestionState QuestionState { get; private set; }

        public MakeFriendsPlayState PlayState { get; private set; }

        public MakeFriendsResultState ResultState { get; private set; }

        public int CurrentScore { get { return friendships; } }

        private readonly int STARS_1_THRESHOLD = Mathf.CeilToInt(0.33f * numberOfRounds);
        private readonly int STARS_2_THRESHOLD = Mathf.CeilToInt(0.66f * numberOfRounds);
        private readonly int STARS_3_THRESHOLD = numberOfRounds;

        public int CurrentStars
        {
            get
            {
                if (CurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new MakeFriendsIntroductionState(this);
            QuestionState = new MakeFriendsQuestionState(this);
            PlayState = new MakeFriendsPlayState(this);
            ResultState = new MakeFriendsResultState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return MakeFriendsConfiguration.Instance;
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        protected override void Start()
        {
            base.Start();

            AppManager.Instance.InitDataAI();
            AppManager.Instance.CurrentGameManagerGO = gameObject;
            SceneTransitioner.Close();
            PlayIdleMusic();

            //Random.seed = System.DateTime.Now.GetHashCode();
            //LoggerEA4S.Log("minigame", "template", "start", "");
            //LoggerEA4S.Save();
        }
            
        public void PlayActiveMusic()
        {
            MakeFriendsConfiguration.Instance.Context.GetAudioManager().PlayMusic(Music.Theme6);
        }

        public void PlayIdleMusic()
        {
            MakeFriendsConfiguration.Instance.Context.GetAudioManager().PlayMusic(Music.Relax);
        }
            
        public void Play()
        {
            currentRound++;
            if (currentRound <= numberOfRounds)
            {
                StartNewRound();
            }
            else
            {
                EndGame();
            }
        }

        public void StartNewRound()
        {
            Reset();
            SetNewWords();
            SetLetterChoices();
            SpawnLivingLetters();
            ShowLetterPicker();
            ShowDropZone();
        }

        private void SetNewWords()
        {
            // Get words with at least 1 common letter
            do
            {
                commonLetters.Clear();
                uncommonLetters.Clear();

                wordData1 = AppManager.Instance.Teacher.GimmeAGoodWordData();
                wordLetters1 = ArabicAlphabetHelper.LetterDataListFromWord(wordData1.Data.Arabic, AppManager.Instance.Letters);

                wordData2 = AppManager.Instance.Teacher.GimmeAGoodWordData();
                wordLetters2 = ArabicAlphabetHelper.LetterDataListFromWord(wordData2.Data.Arabic, AppManager.Instance.Letters);

                // Find common letters (without repetition)
                for (int i = 0; i < wordLetters1.Count; i++)
                {
                    var letter = wordLetters1[i];

                    if (wordLetters2.Contains(letter))
                    {
                        if (!commonLetters.Contains(letter))
                        {
                            commonLetters.Add(letter);
                        }
                    }
                }

                // Find uncommon letters (without repetition)
                for (int i = 0; i < wordLetters1.Count; i++)
                {
                    var letter = wordLetters1[i];

                    if (!wordLetters2.Contains(letter))
                    {
                        if (!uncommonLetters.Contains(letter))
                        {
                            uncommonLetters.Add(letter);
                        }
                    }
                }
                for (int i = 0; i < wordLetters2.Count; i++)
                {
                    var letter = wordLetters2[i];

                    if (!wordLetters1.Contains(letter))
                    {
                        if (!uncommonLetters.Contains(letter))
                        {
                            uncommonLetters.Add(letter);
                        }
                    }
                }

                //Debug.Log("Words: " + wordData1.Word + ", " + wordData2.Word + " " + (commonLetters.Count > 0 ? "Using them" : "Retrying"));
            } while(commonLetters.Count == 0);
                
            commonLetters.Shuffle();
            uncommonLetters.Shuffle();
            Debug.Log("Common letters: " + commonLetters.Count + " Uncommon letters: " + uncommonLetters.Count);
        }

        private void SetLetterChoices()
        {
            choiceLetters.AddRange(commonLetters);
            if (choiceLetters.Count > letterPicker.letterChoices.Length)
            {
                choiceLetters = choiceLetters.GetRange(0, letterPicker.letterChoices.Length);
            }
            //Debug.Log("Added " + choiceLetters.Count + " common letters to choices");
                
            int vacantChoiceLettersCount = letterPicker.letterChoices.Length - choiceLetters.Count;

            // Get other random letters (without repetition)
            for (int i = 0; i < vacantChoiceLettersCount; i++)
            {
                LL_LetterData letter;
                do
                {
                    if (i < uncommonLetters.Count)
                    {
                        letter = uncommonLetters[i];
                        //Debug.Log("Considering as choice: " + letter.TextForLivingLetter);
                        if (choiceLetters.Contains(letter))
                        {
                            letter = AppManager.Instance.Letters.GetRandomElement();
                            //Debug.Log("Using random choice instead: " + letter);
                        }
                    }
                    else
                    {
                        letter = AppManager.Instance.Letters.GetRandomElement();
                        //Debug.Log("No more word letters, using random: " + letter.TextForLivingLetter);
                    }
                } while (choiceLetters.Contains(letter));
                choiceLetters.Add(letter);
                //Debug.Log("Added " + letter.TextForLivingLetter + " to choices");
            }
            choiceLetters.Shuffle();

            letterPicker.DisplayLetters(choiceLetters);
        }

        private void SpawnLivingLetters()
        {
            leftArea.SpawnLivingLetter(wordData1);
            rightArea.SpawnLivingLetter(wordData2);

            leftArea.MakeEntrance();
            rightArea.MakeEntrance();
        }

        private void ShowDropZone()
        {
            dropZone.Appear(uiDelay);
        }

        private void HideDropZone()
        {
            dropZone.Disappear();
        }

        private void ShowLetterPicker()
        {
            letterPicker.Block();
            letterPicker.ShowAndUnblockDelayed(uiDelay);
        }

        private void HideLetterPicker()
        {
            letterPicker.Block();
            letterPicker.Hide();
        }

        public void OnRoundResultPressed()
        {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            WidgetPopupWindow.I.Close();
            Play();
        }

        public void OnLetterChoiceSelected(LetterChoiceController letterChoice)
        {
            letterPicker.BlockForSeconds(2f);

            if (commonLetters.Contains(letterChoice.letterData))
            {
                letterChoice.State = LetterChoiceController.ChoiceState.CORRECT;
                //letterChoice.SpawnBalloon(true);
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
                dropZone.AnimateCorrect();

                if (!correctChoices.Contains(letterChoice.letterData))
                {
                    correctChoices.Add(letterChoice.letterData);
                }

                if (correctChoices.Count >= commonLetters.Count)
                {
                    EndRound(true);
                }
                else
                {
                    dropZone.ResetLetter(3f);
                }
            }
            else
            {
                letterChoice.State = LetterChoiceController.ChoiceState.WRONG;
                //letterChoice.SpawnBalloon(false);
                AudioManager.I.PlaySfx(Sfx.LetterSad);
                dropZone.AnimateWrong();
                leftArea.MoveAwayAngrily();
                rightArea.MoveAwayAngrily();

                incorrectChoices.Add(letterChoice.letterData);
                if (incorrectChoices.Count >= 3)
                {
                    EndRound(false);
                }
            }
        }

        private void EndRound(bool win)
        {
            StartCoroutine(EndRound_Coroutine(win));
        }

        private IEnumerator EndRound_Coroutine(bool win)
        {
            var winDelay1 = 2f;
            var winDelay2 = 1.5f;
            var friendlyExitDelay = leftArea.friendlyExitDuration;
            var loseDelay = 1.5f;

            HideLetterPicker();

            if (win)
            {
                Debug.Log("Win");

                AudioManager.I.PlaySfx(Sfx.Win);
                leftArea.Celebrate();
                rightArea.Celebrate();
                friendships++;

                yield return new WaitForSeconds(winDelay1);
                // Go to Friends Zone
                // ...
                leftArea.MakeFriendlyExit();
                rightArea.MakeFriendlyExit();
                yield return new WaitForSeconds(friendlyExitDelay);
                leftArea.GoToFriendsZone(FriendsZonesManager.instance.currentZone);
                rightArea.GoToFriendsZone(FriendsZonesManager.instance.currentZone);
                FriendsZonesManager.instance.IncrementCurrentZone();
                yield return new WaitForSeconds(winDelay2);
                HideDropZone();
                WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, "comment_welldone", true, null);
            }
            else
            {
                Debug.Log("Lose");

                AudioManager.I.PlaySfx(Sfx.Lose);
                yield return new WaitForSeconds(loseDelay);
                HideDropZone();
                WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, "game_balloons_commentA", false, null);
            }
        }

        private void Reset()
        {
            commonLetters.Clear();
            choiceLetters.Clear();
            correctChoices.Clear();
            incorrectChoices.Clear();
            wordData1 = null;
            wordData2 = null;
            wordLetters1.Clear();
            wordLetters2.Clear();

            letterPicker.Reset();
            dropZone.Reset();
            leftArea.Reset();
            rightArea.Reset();
        }

        private void EndGame()
        {
            StartCoroutine(EndGame_Coroutine());
        }

        private IEnumerator EndGame_Coroutine()
        {
            var delay1 = 1f;
            yield return new WaitForSeconds(delay1);

            PlayIdleMusic();
            Reset();

            // Zoom out camera
            var fromPosition = sceneCamera.transform.localPosition;
            var toPosition = endCameraPosition;
            var fromRotation = sceneCamera.transform.localRotation.eulerAngles;
            var toRotation = endCameraRotation;
            var interpolant = 0f;
            var lerpProgress = 0f;
            var lerpLength = 2f;

            while (lerpProgress < lerpLength)
            {
                sceneCamera.transform.localPosition = Vector3.Lerp(fromPosition, toPosition, interpolant);
                sceneCamera.transform.localRotation = Quaternion.Euler(Vector3.Lerp(fromRotation, toRotation, interpolant));
                lerpProgress += Time.deltaTime;
                interpolant = lerpProgress / lerpLength;
                interpolant = Mathf.Sin(interpolant * Mathf.PI * 0.5f);
                yield return new WaitForFixedUpdate();
            }
                
//            endGameCanvas.gameObject.SetActive(true);
//
//            int numberOfStars = 0;
//
//            if (friendships <= 0)
//            {
//                numberOfStars = 0;
//                WidgetSubtitles.I.DisplaySentence("game_result_retry");
//            }
//            else if ((float)friendships / numberOfRounds < 0.5f)
//            {
//                numberOfStars = 1;
//                WidgetSubtitles.I.DisplaySentence("game_result_fair");
//            }
//            else if (friendships < numberOfRounds)
//            {
//                numberOfStars = 2;
//                WidgetSubtitles.I.DisplaySentence("game_result_good");
//            }
//            else
//            {
//                numberOfStars = 3;
//                WidgetSubtitles.I.DisplaySentence("game_result_great");
//            }
//            starFlowers.Show(numberOfStars);

            PlayState.OnResult();
        }
    }


    [Serializable]
    public class MakeFriendsGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 0f;
    }
}
