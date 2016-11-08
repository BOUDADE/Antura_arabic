﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using EA4S.API;
using System.Collections.Generic;

namespace EA4S.Test
{

    public class MiniGameLauncher : MonoBehaviour
    {

        public MiniGamesDropDownList MiniGamesDropDownList;
        public Button LaunchButton;

        public void LaunchGame()
        {
            // Example minigame call
            MiniGameCode miniGameCodeSelected = (MiniGameCode)Enum.Parse(typeof(MiniGameCode), MiniGamesDropDownList.options[MiniGamesDropDownList.value].text);
            float difficulty = float.Parse(FindObjectsOfType<InputField>().First(n => n.name == "Difficulty").text);
           
            // Call start game with parameters
            MiniGameAPI.Instance.StartGame(
                miniGameCodeSelected,
                new GameConfiguration(difficulty)
            );

        }

        #region Test Helpers

        LL_WordData GetWord()
        {
            return AppManager.Instance.Teacher.GimmeAGoodWordData();
        }

        List<LL_WordData> GetWordsNotContained(List<LL_WordData> _WordsToAvoid, int _count)
        {
            List<LL_WordData> wordListToReturn = new List<LL_WordData>();
            for (int i = 0; i < _count; i++) {
                var word = AppManager.Instance.Teacher.GimmeAGoodWordData();

                if (!CheckIfContains(_WordsToAvoid, word) && !CheckIfContains(wordListToReturn, word)) {
                    wordListToReturn.Add(word);
                }
            }
            return wordListToReturn;
        }

        List<LL_LetterData> GetLettersFromWord(LL_WordData _word)
        {
            List<LL_LetterData> letters = new List<LL_LetterData>();
            foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(_word.Data.Arabic, AppManager.Instance.Letters)) {
                letters.Add(letterData);
            }
            return letters;
        }

        List<LL_LetterData> GetLettersNotContained(List<LL_LetterData> _lettersToAvoid, int _count)
        {
            List<LL_LetterData> letterListToReturn = new List<LL_LetterData>();
            for (int i = 0; i < _count; i++) {
                var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                if (!CheckIfContains(_lettersToAvoid, letter) && !CheckIfContains(letterListToReturn, letter)) {
                    letterListToReturn.Add(letter);
                }
            }
            return letterListToReturn;
        }


        static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }


        static bool CheckIfContains(List<LL_LetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        static bool CheckIfContains(List<LL_WordData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        #endregion
    }
}
