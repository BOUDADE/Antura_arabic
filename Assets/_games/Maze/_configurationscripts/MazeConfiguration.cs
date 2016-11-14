﻿namespace EA4S.Maze {
    public enum MazeVariation : int {
        V_1 = 1,
    }

    public class MazeConfiguration : IGameConfiguration {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        #region Game configurations
        public float Difficulty { get; set; }
        public MazeVariation Variation { get; set; }
        #endregion

        /////////////////
        // Singleton Pattern
        static MazeConfiguration instance;
        public static MazeConfiguration Instance {
            get {
                if (instance == null)
                    instance = new MazeConfiguration();
                return instance;
            }
        }
        /////////////////

        private MazeConfiguration() {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();

            Variation = MazeVariation.V_1;

            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new MazeConfiguration() {
                Difficulty = _difficulty,
                Variation = (MazeVariation)_variation,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 5;

            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect);

            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }

    }
}
