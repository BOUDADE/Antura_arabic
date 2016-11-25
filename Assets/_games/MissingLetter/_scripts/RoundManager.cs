﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;
using System;

namespace EA4S.MissingLetter
{

    public class RoundManager
    {

        #region API
        public RoundManager(MissingLetterGame _game)
        {
            m_oGame = _game;

            //if(m_oGame.m_eGameType == GameType.WORD)
            //{
            //    MissingLetterConfiguration.Instance.PipeQuestions = new SampleQuestionProvider();
            //}
            //else
            //{
            //}
        }

        public void Initialize()
        {
            m_oCurrQuestionPack = null;

            //set the answers and questions center position
            //TODO remove magic number 20, distance beetween camera and LL
            m_v3QstPos = m_oGame.m_oQuestionCamera.position + new Vector3(0, m_oGame.m_sQuestionOffset.fHeightOffset, 20);
            m_v3AnsPos = m_oGame.m_oAnswerCamera.position + new Vector3(0, m_oGame.m_sAnswerOffset.fHeightOffset, 20);
            m_oGame.m_oLetterPrefab.GetComponent<LetterBehaviour>().mfDistanceBetweenLetters = m_oGame.m_fDistanceBetweenLetters;

            int qstPoolSize = 3;
            qstPoolSize *= (m_oGame.m_eGameType == GameType.WORD) ? 1 : m_oGame.m_iMaxSentenceSize;
            m_oGame.m_oLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(m_v3QstPos + Vector3.right * m_oGame.m_sQuestionOffset.fINOffset, m_v3QstPos, m_v3QstPos + Vector3.right * m_oGame.m_sQuestionOffset.fOUTOffset);
            m_oQuestionPool = new GameObjectPool(m_oGame.m_oLetterPrefab, qstPoolSize, false);

            int ansPoolSize = m_oGame.m_iNumberOfPossibleAnswers * 3;
            m_oGame.m_oLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(m_v3AnsPos + Vector3.right * m_oGame.m_sAnswerOffset.fINOffset, m_v3AnsPos, m_v3AnsPos + Vector3.right * m_oGame.m_sAnswerOffset.fOUTOffset);
            m_oAnswerPool = new GameObjectPool(m_oGame.m_oLetterPrefab, ansPoolSize, false);

            m_oEmoticonsController = new MissingLetterEmoticonsController(m_oGame.m_oEmoticonsController);
        }

        public void SetTutorial(bool _enabled) {
            this.m_bTutorialEnabled = _enabled;
        }

        public void NewRound()
        {
            m_oGame.SetInIdle(false);
            ExitCurrentScene();

            if (m_oGame.m_eGameType == GameType.WORD)
            {
                NextWordQuestion();
            }
            else
            {
                NextSentenceQuestion();
            }

            if (m_oGame.GetCurrentState() == m_oGame.PlayState || m_bTutorialEnabled)
            {
                EnterCurrentScene();
                m_oGame.StartCoroutine(Utils.LaunchDelay(2.0f, m_oGame.SetInIdle, true));
            }
        }

        public void Terminate()
        {
            if(m_oGame.m_iCurrentRound < m_oGame.m_iRoundsLimit)
                ExitCurrentScene();
        }

        public GameObject GetCorrectLLObject()
        {
            return m_oCorrectAnswer;
        }

        public void OnAnswerClicked(string _key)
        {
            m_oGame.SetInIdle(false);

            //refresh the data (for graphics)
            RestoreQuestion(isCorrectAnswer(_key));

            foreach (GameObject _obj in m_aoCurrentAnswerScene)
            {
                _obj.GetComponent<LetterBehaviour>().SetEnabledCollider(false);
            }

            //letter animation wait for ending dancing animation, wait animator fix
            LetterBehaviour clicked = GetAnswerById(_key);
            if (isCorrectAnswer(_key))
            {
                clicked.PlayAnimation(LLAnimationStates.LL_still);
                clicked.mLetter.DoHorray();
                clicked.LightOn();

                PlayPartcileSystem(m_aoCurrentQuestionScene[0].transform.position + Vector3.up * 2);

                m_oGame.StartCoroutine(Utils.LaunchDelay(0.5f, OnResponse, true));
            }
            else
            {
                clicked.PlayAnimation(LLAnimationStates.LL_still);
                OnResponse(false);
            }
        }

        //shuffle current answer order and tell to letter change pos
        public void ShuffleLetters(float duration)
        {
            if (m_oGame.IsInIdle())
            {
                m_oGame.SetInIdle(false);
                m_aoCurrentAnswerScene.Shuffle();
                for (int i = 0; i < m_aoCurrentAnswerScene.Count; ++i)
                {
                    float offsetDuration = UnityEngine.Random.Range(-2.0f, 0.0f);
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().ChangePos(i, m_aoCurrentAnswerScene.Count, duration + offsetDuration);
                }
                m_oGame.StartCoroutine(Utils.LaunchDelay(duration, m_oGame.SetInIdle, true));
            }
        }

        #endregion

        #region PRIVATE_FUNCTION

        void NextWordQuestion() {

            m_iRemovedLLDataIndex = 0;

            m_oCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            ILivingLetterData questionData = m_oCurrQuestionPack.GetQuestion();

            var _wrongAnswers = m_oCurrQuestionPack.GetWrongAnswers().ToList();
            var _correctAnswer = m_oCurrQuestionPack.GetCorrectAnswers().ToList()[0];

            GameObject oQuestion = m_oQuestionPool.GetElement();

            LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();
            qstBehaviour.Reset();
            qstBehaviour.LetterData = questionData;
            qstBehaviour.endTransformToCallback += qstBehaviour.Speak;
            qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;
            qstBehaviour.m_oDefaultIdleAnimation = LLAnimationStates.LL_idle;
            m_aoCurrentQuestionScene.Add(oQuestion);

            m_oEmoticonsController.init(qstBehaviour.transform);

            //after insert in mCurrentQuestionScene
            RemoveLetterfromQuestion();

            GameObject _correctAnswerObject = m_oAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();
            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswer;
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            corrAnsBheaviour.onLetterClick += OnAnswerClicked;

            corrAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

            m_aoCurrentAnswerScene.Add(_correctAnswerObject);
            m_oCorrectAnswer = _correctAnswerObject;

            for (int i = 1; i < m_oGame.m_iNumberOfPossibleAnswers && i < _wrongAnswers.Count()+1; ++i) {
                GameObject _wrongAnswerObject = m_oAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i-1);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

                if(!m_bTutorialEnabled)
                {
                    wrongAnsBheaviour.onLetterClick += OnAnswerClicked;
                }
                wrongAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

                m_aoCurrentAnswerScene.Add(_wrongAnswerObject);
            }

            m_aoCurrentAnswerScene.Shuffle();
        }

        List<LL_WordData> GetWordFromPhrases(LL_PhraseData _phrase)
        {
            List<LL_WordData> phrase = new List<LL_WordData>();
            for(int i=0; i < m_oGame.m_iMaxSentenceSize; ++i)
            {
                phrase.Add(AppManager.Instance.Teacher.GetRandomTestWordDataLL());
            }

            return phrase;
        }

        void NextSentenceQuestion()
        {
            m_oCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            
            //tmp for test
            //LL_PhraseData questionData = m_oCurrQuestionPack.GetQuestion();
            //var words = ArabicAlphabetHelper.WordDataListFromPhrase(questionData);
            //var _wrongAnswers = m_oCurrQuestionPack.GetWrongAnswers();
            //var _correctAnswers = m_oCurrQuestionPack.GetCorrectAnswers();

            List<LL_WordData> questionData = GetWordFromPhrases(null);
            var _correctAnswer = questionData[0];

            var _wrongAnswers = new List<LL_WordData>();
            for (int i = 0; i < m_oGame.m_iMaxSentenceSize; ++i)
            {
                _wrongAnswers.Add(AppManager.Instance.Teacher.GetRandomTestWordDataLL());
            }
            _wrongAnswers.Distinct();


            foreach (LL_WordData _word in questionData)
            {
                GameObject oQuestion = m_oQuestionPool.GetElement();
                LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();

                qstBehaviour.Reset();
                qstBehaviour.LetterData = _word;
                qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;
                qstBehaviour.m_oDefaultIdleAnimation = LLAnimationStates.LL_idle;

                m_aoCurrentQuestionScene.Add(oQuestion);
            }

            //after insert in mCurrentQuestionScene
            m_iRemovedLLDataIndex = RemoveWordfromQuestion(questionData);
            m_aoCurrentQuestionScene[m_iRemovedLLDataIndex].GetComponent<LetterBehaviour>().endTransformToCallback += m_aoCurrentQuestionScene[m_iRemovedLLDataIndex].GetComponent<LetterBehaviour>().Speak;

            GameObject _correctAnswerObject = m_oAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();

            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswer;
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            corrAnsBheaviour.onLetterClick += OnAnswerClicked;

            corrAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

            m_aoCurrentAnswerScene.Add(_correctAnswerObject);
            m_oCorrectAnswer = _correctAnswerObject;

            for (int i = 1; i < m_oGame.m_iNumberOfPossibleAnswers && i < _wrongAnswers.Count() + 1; ++i)
            {
                GameObject _wrongAnswerObject = m_oAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i - 1);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

                if (!m_bTutorialEnabled)
                {
                    wrongAnsBheaviour.onLetterClick += OnAnswerClicked;
                }
                wrongAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

                m_aoCurrentAnswerScene.Add(_wrongAnswerObject);
            }

            m_aoCurrentAnswerScene.Shuffle();

            m_oEmoticonsController.init(m_aoCurrentQuestionScene[m_iRemovedLLDataIndex].transform);

        }

        void RemoveLetterfromQuestion()
        {
            LL_WordData word = (LL_WordData)m_oCurrQuestionPack.GetQuestion();
            var Letters = ArabicAlphabetHelper.LetterDataListFromWord(word.Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL());

            LL_LetterData letter = (LL_LetterData)m_oCurrQuestionPack.GetCorrectAnswers().ToList()[0];
            int index = 0;
            for (; index < Letters.Count; ++index)
            {
                if (Letters[index].Id == letter.Id)
                {
                    break;
                }
            }

            LetterObjectView tmp = m_aoCurrentQuestionScene[0].GetComponent<LetterBehaviour>().mLetter;
            tmp.Label.text = tmp.Label.text.Remove(index, 1);
            tmp.Label.text = tmp.Label.text.Insert(index, mk_sRemovedLetterChar);

        }

        
        //Preparing for missingWord
        int RemoveWordfromQuestion(List<LL_WordData> Words)
        {
            //tmp solution for testing misingWord
            LL_WordData word = Words[0];

            int index = 0;
            for (; index < Words.Count; ++index)
            {
                if (Words[index].Id == word.Id)
                {
                    break;
                }
            }

            LetterObjectView tmp = m_aoCurrentQuestionScene[index].GetComponent<LetterBehaviour>().mLetter;
            tmp.Label.text = mk_sRemovedLetterChar;
            return index;
        }
        

        void RestoreQuestion(bool result)
        {
            LetterObjectView tmp = m_aoCurrentQuestionScene[m_iRemovedLLDataIndex].GetComponent<LetterBehaviour>().mLetter;
            int index = tmp.Label.text.IndexOf(mk_sRemovedLetterChar);

            foreach (GameObject _obj in m_aoCurrentQuestionScene)
            {
                _obj.GetComponent<LetterBehaviour>().Refresh();
            }

            if(result)
                m_oEmoticonsController.EmoticonPositive();
            else
                m_oEmoticonsController.EmoticonNegative();
            
            //change restored color letter with tag
            string color = result ? "#4CAF50" : "#DD2C00";

            string first;
            if (m_oGame.m_eGameType == GameType.WORD)
            {
                first = tmp.Label.text[index].ToString();
            }
            else
            {
                first = tmp.Label.text;
            }

            tmp.Label.text = tmp.Label.text.Replace(first, "<color="+ color + ">" + first + "</color>");
        }

        void EnterCurrentScene() {
            int _pos = 0;
            foreach (GameObject _obj in m_aoCurrentQuestionScene) {
                _obj.GetComponent<LetterBehaviour>().EnterScene(_pos, m_aoCurrentQuestionScene.Count());
                ++_pos;
            }

            _pos = 0;
            foreach (GameObject _obj in m_aoCurrentAnswerScene) {
                _obj.GetComponent<LetterBehaviour>().EnterScene( _pos, m_aoCurrentAnswerScene.Count());
                ++_pos;
            }
        }

        void ExitCurrentScene() {
            if (m_oCurrQuestionPack != null) {

                foreach (GameObject _obj in m_aoCurrentQuestionScene) {
                    _obj.GetComponent<LetterBehaviour>().ExitScene();
                }
                m_aoCurrentQuestionScene.Clear();

                foreach (GameObject _obj in m_aoCurrentAnswerScene) {
                    _obj.GetComponent<LetterBehaviour>().ExitScene();
                }
                m_aoCurrentAnswerScene.Clear();
            }
        }


        void OnQuestionLetterBecameInvisible(GameObject _obj) {
            m_oQuestionPool.FreeElement(_obj);
            _obj.GetComponent<LetterBehaviour>().onLetterBecameInvisible -= OnQuestionLetterBecameInvisible;
        }

        void OnAnswerLetterBecameInvisible(GameObject _obj) {
            m_oAnswerPool.FreeElement(_obj);
            _obj.GetComponent<LetterBehaviour>().onLetterBecameInvisible -= OnAnswerLetterBecameInvisible;
        }

        private bool isCorrectAnswer(string _id)
        {
            return m_oCorrectAnswer.GetComponent<LetterBehaviour>().mLetterData.Id == _id;
        }

        private LetterBehaviour GetAnswerById(string _key)
        {
            foreach (GameObject _obj in m_aoCurrentAnswerScene)
            {
                if (_obj.GetComponent<LetterBehaviour>().mLetterData.Id == _key)
                    return _obj.GetComponent<LetterBehaviour>();
            }
            return null;
        }

        //call after clicked answer animation
        private void OnResponse(bool correct)
        {
            if (correct)
            {
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
                DoWinAnimations();
            }
            else
            {
                AudioManager.I.PlaySfx(Sfx.LetterSad);
                DoLoseAnimations();
            }

            if (onAnswered != null)
            {
                m_oGame.StartCoroutine(Utils.LaunchDelay(2.0f, onAnswered, correct));
            }
        }

        private void PlayPartcileSystem(Vector3 _pos)
        {
            m_oGame.m_oParticleSystem.SetActive(true);
            m_oGame.m_oParticleSystem.transform.position = _pos;
            m_oGame.m_oParticleSystem.GetComponent<ParticleSystem>().Play();
            m_oGame.StartCoroutine(Utils.LaunchDelay(1.5f, delegate { m_oGame.m_oParticleSystem.SetActive(false); }));
        }

        //win animation: quesion high five, correct answer dancing other horray
        private void DoWinAnimations()
        {
            for (int i = 0; i < m_aoCurrentAnswerScene.Count; ++i)
            {
                if(isCorrectAnswer(m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Id))
                {
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoDancingWin();
                }
                else
                {
                    //random delay poof of wrong answer
                    m_oGame.StartCoroutine(Utils.LaunchDelay(UnityEngine.Random.Range(0, 0.5f), PoofLetter, m_aoCurrentAnswerScene[i]));
                }
            }

            for (int i = 0; i < m_aoCurrentQuestionScene.Count; ++i)
            {
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoHighFive();
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().LightOn();
            }
        }

        //poof the gameobject letter
        private void PoofLetter(GameObject letter)
        {
            if (!letter.GetComponent<LetterBehaviour>())
            {
                Debug.LogWarning("Cannot poof letter " + letter.name);
                return;
            }

            letter.GetComponent<LetterBehaviour>().mLetter.Poof();
            letter.transform.position = Vector3.zero;
            AudioManager.I.PlaySfx(Sfx.Poof);
        }

        //lose animation: quesion and correct answer angry other crouch
        private void DoLoseAnimations()
        {
            for (int i = 0; i < m_aoCurrentQuestionScene.Count; ++i)
            {
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoAngry();
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().LightOn();
            }

            for (int i = 0; i < m_aoCurrentAnswerScene.Count; ++i)
            {
                if (m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Id == m_oCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Id)
                {
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoAngry();
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LightOn();
                }
                else
                {
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.Crouching = true;
                }
            }
        }
        #endregion

        #region VARS
        private const string mk_sRemovedLetterChar = "_";

        private MissingLetterGame m_oGame;

        private int m_iRemovedLLDataIndex;

        private IQuestionPack m_oCurrQuestionPack;

        private GameObjectPool m_oAnswerPool;
        private GameObjectPool m_oQuestionPool;

        private List<GameObject> m_aoCurrentQuestionScene = new List<GameObject>();
        private List<GameObject> m_aoCurrentAnswerScene = new List<GameObject>();
        private GameObject m_oCorrectAnswer;

        private Vector3 m_v3AnsPos;
        private Vector3 m_v3QstPos;

        public event Action<bool> onAnswered;

        private MissingLetterEmoticonsController m_oEmoticonsController;

        private bool m_bTutorialEnabled;

        #endregion

    }
}