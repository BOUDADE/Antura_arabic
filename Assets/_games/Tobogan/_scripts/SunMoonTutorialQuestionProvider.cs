﻿namespace EA4S.Tobogan
{
    public class SunMoonTutorialQuestionProvider : IQuestionProvider
    {
        IQuestionProvider provider;

        IQuestionPack sunQuestion;
        IQuestionPack moonQuestion;
        int questionsDone = 0;

        public SunMoonTutorialQuestionProvider(IQuestionProvider provider)
        {
            this.provider = provider;

            var db = AppManager.Instance.DB;
            var sunWord = db.GetWordDataById("the_sun");
            var sunData = new LL_WordData(sunWord.Id, sunWord);
            var moonWord = db.GetWordDataById("the_moon");
            var moonData = new LL_WordData(moonWord.Id, moonWord);

            sunQuestion = new SampleQuestionPack(sunData, new ILivingLetterData[] { moonData }, new ILivingLetterData[] { sunData });
            moonQuestion = new SampleQuestionPack(moonData, new ILivingLetterData[] { sunData }, new ILivingLetterData[] { moonData });
        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            questionsDone++;

            if (questionsDone == 1)
                return sunQuestion;
            else if (questionsDone == 2)
                return moonQuestion;
            else
                return provider.GetNextQuestion();
        }
    }
}
