﻿namespace EA4S.MissingLetter
{
    public class MissingLetterConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public IQuestionProvider PipeQuestions { get; set; }

        public float Difficulty { get; set; }

        /////////////////
        // Singleton Pattern
        static MissingLetterConfiguration instance;
        public static MissingLetterConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MissingLetterConfiguration();
                return instance;
            }
        }
        /////////////////

        private MissingLetterConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            PipeQuestions = new SampleQuestionProvider();

            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            builder = new LettersInWordQuestionBuilder(nPacks, nCorrect, nWrong);

            return builder;
        }
    }
}
