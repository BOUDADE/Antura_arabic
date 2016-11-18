﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.HideAndSeek
{
	public class HideAndSeekGameManager : MonoBehaviour {
		void OnEnable()
        {
			HideAndSeekTreeController.onTreeTouched += MoveObject;
			HideAndSeekLetterController.onLetterTouched += CheckResult;
		}
		void OnDisable()
        {
			HideAndSeekTreeController.onTreeTouched -= MoveObject;
			HideAndSeekLetterController.onLetterTouched -= CheckResult;
		}
		
		void Start ()
        {
            for(int i = 0; i < MAX_OBJECT; ++i)
            {
                UsedPlaceholder[i] = false;
            }
        }
	
		void Update ()
        {
            if (StartNewRound && game.inGame && Time.time > time + timeToWait)
            {
                NewRound();
            }
        }

		void MoveObject(int id){
            if (ArrayLetters.Length > 0)
            {
                script = ArrayLetters[GetIdFromPosition(id)].GetComponent<HideAndSeekLetterController>();
                script.Move();
            }
		}

        int GetIdFromPosition(int index)
        {
            for(int i = 0; i < ArrayLetters.Length; ++i)
            {
                if (ArrayLetters[i].GetComponent<HideAndSeekLetterController>().id == index)
                    return i;
            }
            return -1;
        }

        /*void NewRoundSetup()
        {
            StartNewRound = true;
            SetTime();
            WidgetPopupWindow.I.Close();
        }*/

        public void RepeatAudio()
        {
            AudioManager.I.PlayLetter(currentQuestion.GetAnswer().Key);
        }

        private IEnumerator DelayAnimation()
        {
            game.PlayState.gameTime.Stop();

            var initialDelay = 3f;
            yield return new WaitForSeconds(initialDelay);

            foreach (GameObject x in ArrayLetters)
            {
                x.GetComponent<LetterObjectView>().Poof();
                AudioManager.I.PlaySfx(Sfx.Poof);
                x.SetActive(false);
            }

            var delay = 0.5f;
            yield return new WaitForSeconds(delay);

            StartNewRound = true;
            SetTime();
        }

        void CheckResult(int id)
		{
            letterInAnimation = GetIdFromPosition(id);
            HideAndSeekLetterController script = ArrayLetters[letterInAnimation].GetComponent<HideAndSeekLetterController>();
            if (script.view.Data.Key == currentQuestion.GetAnswer().Key)
            {
                LockTrees();
                StartCoroutine(DelayAnimation());
                script.resultAnimation(true);
                game.OnResult();
                buttonRepeater.SetActive(false);
                AudioManager.I.PlaySfx(Sfx.Win);
            }
            else
            {
                RemoveLife();
                script.resultAnimation(false);
                if (lifes == 0)
                {
                    LockTrees();
                    AudioManager.I.PlaySfx(Sfx.Lose);
                    StartCoroutine(DelayAnimation());
                    buttonRepeater.SetActive(false);
                }
            }
        }

        void RemoveLife()
        {
            switch (--lifes)
            {
                case 2:
                    game.Context.GetOverlayWidget().SetLives(2);
                    break;
                case 1:
                    game.Context.GetOverlayWidget().SetLives(1);
                    break;
                case 0:
                    game.Context.GetOverlayWidget().SetLives(0);
                    break;
            }
        }

        void SetFullLife()
        {
            lifes = 3;
            game.Context.GetOverlayWidget().SetLives(3);
        }

        public void SetTime()
        {
            time = Time.time;
        }

        public void LockTrees()
        {
            for (int i = 0; i < MAX_OBJECT; ++i)
            {
                ArrayTrees[i].GetComponent<CapsuleCollider>().enabled = false;
            }
        }
        public void ClearRound()
        {
            for(int i = 0; i < MAX_OBJECT; ++i)
            {
                ArrayLetters[i].SetActive(true);
                ArrayLetters[i].transform.position = originLettersPlaceholder.position;
                ArrayLetters[i].GetComponent<HideAndSeekLetterController>().ResetLetter();
                UsedPlaceholder[i] = false;
            }
        }

        public void NewRound()
        {
            ClearRound();

            currentQuestion = (HideAndSeekQuestionsPack)questionProvider.GetQuestion();
            StartNewRound = false;
            SetFullLife();
            FreePlaceholder = MAX_OBJECT;
            ActiveLetters = currentQuestion.GetLetters().Count;

            ActiveTrees = new List<GameObject>();

            List<ILivingLetterData> letterList = currentQuestion.GetLetters();

            for(int i = 0; i < ActiveLetters; ++i)
            {
                int index = getRandomPlaceholder();
                if(index != -1)
                {

                    ActiveTrees.Add(ArrayTrees[index]);
       
                    ArrayLetters[i].transform.position = ArrayPlaceholder[index].transform.position;
                    HideAndSeekLetterController scriptComponent = ArrayLetters[i].GetComponent<HideAndSeekLetterController>();
                    scriptComponent.SetStartPosition(ArrayPlaceholder[index].transform.position);
                    scriptComponent.id = index;
                    SetLetterMovement(index, scriptComponent);
                    ArrayLetters[i].GetComponentInChildren<LetterObjectView>().Init(letterList[i]);
                }
            }
            StartCoroutine(DisplayRound_Coroutine());

        }

        public void SetLetterMovement( int placeholder, HideAndSeekLetterController script)
        {
            if (placeholder == 1)
                script.SetMovement(MovementType.OnlyRight);
            else if(placeholder == 2)
                script.SetMovement(MovementType.OnlyLeft);
            else if(placeholder == 0 || placeholder == 6)
                script.SetMovement(MovementType.Enhanced);
            else
                script.SetMovement(MovementType.Normal);
        }
        
        private IEnumerator DisplayRound_Coroutine()
        {
            foreach(GameObject tree in ActiveTrees)
            {
                tree.GetComponent<CapsuleCollider>().enabled = true;
            }

            var winInitialDelay = 0.5f;
            yield return new WaitForSeconds(winInitialDelay);

            AudioManager.I.PlayLetter(currentQuestion.GetAnswer().Key);
            game.PlayState.gameTime.Start();

            buttonRepeater.SetActive(true);
        }

        public int getRandomPlaceholder()
        {
            int result = 0;
            int position = Random.Range(0, FreePlaceholder--);
            
            for(int i = 0; i < UsedPlaceholder.Length; ++i)
            {
                if (UsedPlaceholder[i] == true)
                    continue;
                if (result == position)
                {
                    UsedPlaceholder[i] = true;
                    return i;
                }
                result++;
            }
            return -1;
        }


        #region VARIABLES
        bool StartNewRound = true;
        int lifes;
        int ActiveLetters;
        private const int MAX_OBJECT = 7;
        private int FreePlaceholder;

		public GameObject[] ArrayTrees;
        private List<GameObject> ActiveTrees;
        
        public Transform[] ArrayPlaceholder;
        private bool[] UsedPlaceholder = new bool[MAX_OBJECT];

        public Transform originLettersPlaceholder;

		public GameObject[] ArrayLetters;

        private int letterInAnimation = -1;
        
		private HideAndSeekLetterController script;

        public HideAndSeekGame game;
        
        public HideAndSeekQuestionsProvider questionProvider;
        public HideAndSeekQuestionsPack currentQuestion;

        public float timeToWait = 1.0f;
        private float time;

        public Sprite image;

        public GameObject buttonRepeater;
        #endregion
    }
}