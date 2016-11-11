﻿namespace EA4S.Balloons
{
    public enum BalloonsVariation : int
    {
        Spelling = 1,
        Words = 2,
        Letter = 3,
        Counting = 4,

    }

    public class BalloonsConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }

        public IQuestionProvider Questions { get; set; }

        #region Game configurations

        public float Difficulty { get; set; }

        public BalloonsVariation Variation { get; set; }

        #endregion

        /////////////////
        // Singleton Pattern
        static BalloonsConfiguration instance;

        public static BalloonsConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new BalloonsConfiguration();
                return instance;
            }
        }

        /////////////////

        private BalloonsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();

            Variation = BalloonsVariation.Spelling;

            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        #region external configuration call

        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new BalloonsConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (BalloonsVariation)_variation,
            };
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 4;
            int nWrong = 4;

            switch (Variation)
            {
                case BalloonsVariation.Counting:
                    builder = new OrderedWordsQuestionBuilder(Db.WordDataCategory.Number);
                    break;
                case BalloonsVariation.Letter:
                    builder = new WordsWithLetterQuestionBuilder(nPacks, nCorrect, nWrong);
                    break;  
                case BalloonsVariation.Spelling:
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong);
                    break;
                case BalloonsVariation.Words:
                    builder = new RandomWordsQuestionBuilder(nPacks, 1, nWrong, true);
                    break;
            }

            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }


        #endregion
    }
}
