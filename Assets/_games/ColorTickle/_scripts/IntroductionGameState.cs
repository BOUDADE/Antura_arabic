﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ModularFramework.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ColorTickle
{
    public class IntroductionGameState : IGameState
    {
        ColorTickleGame game;

        float timer = 1f;


        public IntroductionGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.ColorTickle_Title);

            game.colorsCanvas.gameObject.SetActive(false);

            game.Context.GetAudioManager().PlayMusic(game.backgroundMusic);

            BuildLetters();

            BuildTutorialLetter();

            game.tutorialUIManager = game.gameObject.GetComponent<TutorialUIManager>();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
				for (int i = 0; i < game.rounds; ++i) {
					game.myLetters[i].gameObject.SetActive (false);
				}
				game.tutorialLetter.gameObject.SetActive(false);

				game.SetCurrentState(game.TutorialState);
            }
        }

        public void UpdatePhysics(float delta)
        {            
        }

        void BuildLetters()
        {
            game.myLetters = new GameObject[game.rounds];

            //LL_LetterData LLdata;

            for (int i = 0; i < game.rounds; ++i)
            {
                game.myLetters[i] = Object.Instantiate(game.letterPrefab);
                game.myLetters[i].SetActive(true);
                // HACK fix for the automatic reset of the color after update at Unity 5.4.2
                game.myLetters[i].GetComponent<LetterObjectView>().Label.color = Color.white;

                //we don't want diacritic letters for our game 
                //game.myLetters[i].GetComponent<LetterObjectView>().Init(AppManager.I.Teacher.GetRandomTestLetterLL(new LetterFilters(excludeDiacritics:true))); //use this for testing
                game.myLetters[i].GetComponent<LetterObjectView>().Init(AppManager.I.Teacher.GetRandomTestLetterLL()); //use this for final game once the provider is setted with filters

               game.myLetters[i].GetComponent<ColorTickle_LLController>().movingToDestination = false;
            }
        }

        void BuildTutorialLetter()
        {
            LL_LetterData LLdata = new LL_LetterData("alef");
            game.tutorialLetter = Object.Instantiate(game.letterPrefab);
            game.tutorialLetter.SetActive(true);
            // HACK fix for the automatic reset of the color after update at Unity 5.4.2
            game.tutorialLetter.GetComponent<LetterObjectView>().Label.color = Color.white;
            game.tutorialLetter.GetComponent<LetterObjectView>().Init(LLdata);
            game.tutorialLetter.GetComponent<ColorTickle_LLController>().movingToDestination = false;
        }

    }
                
}