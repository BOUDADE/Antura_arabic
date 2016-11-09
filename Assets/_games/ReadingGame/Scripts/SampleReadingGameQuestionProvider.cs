﻿using System.Collections.Generic;

namespace EA4S
{
    public class SampleReadingGameQuestionProvider : IQuestionProvider
    {
        public SampleReadingGameQuestionProvider()
        {

        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            var answerData = AppManager.Instance.DB.GetWordDataByRandom();
            LL_WordData randomWord = new LL_WordData(answerData.Id, answerData);

            StringTestData fakeData = new StringTestData(
                 ArabicAlphabetHelper.PrepareStringForDisplay(
                     "لم نرك منذ مدة " + randomWord.Data.Arabic + " منذ مدة"));

            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            while (wrongAnswers.Count < 6)
            {
                var randomData = AppManager.Instance.DB.GetWordDataByRandom();

                if (randomData.Id != answerData.Id)
                {
                    wrongAnswers.Add(randomData.ConvertToLivingLetterData());
                }
            }
            
            return new SampleQuestionPack(fakeData, wrongAnswers, new ILivingLetterData[] { randomWord });
        }
    }
}
