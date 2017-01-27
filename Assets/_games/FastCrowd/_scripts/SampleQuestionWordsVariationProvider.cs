﻿using System.Collections.Generic;
using EA4S.Core;
using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;

namespace EA4S.Minigames.FastCrowd
{
    /// <summary>
    /// This sample class generates 32 quizzes of type "I give you a word, you say that word"
    /// </summary>
    public class SampleQuestionWordsVariationProvider : IQuestionProvider
    {
        List<SampleQuestionPack> questions = new List<SampleQuestionPack>();

        int currentQuestion;

        public SampleQuestionWordsVariationProvider()
        {
            currentQuestion = 0;

            for (int i = 0; i < 32; i++)
            {
                List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
                List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

                for (int j=0; j<3; ++j)
                {
                    LL_WordData newWordData = AppManager.I.Teacher.GetRandomTestWordDataLL();

                    correctAnswers.Add(newWordData);
                }

                // At least 4 wrong words
                while (wrongAnswers.Count < 4)
                {
                    var word = AppManager.I.Teacher.GetRandomTestWordDataLL();

                    if (!correctAnswers.Contains(word) && !wrongAnswers.Contains(word))
                    {
                        wrongAnswers.Add(word);
                    }
                }

                var currentPack = new SampleQuestionPack(null, wrongAnswers, correctAnswers);
                questions.Add(currentPack);
            }
        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }
}
