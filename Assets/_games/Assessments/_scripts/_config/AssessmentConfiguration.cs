using System;
using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Assessment
{
    public class AssessmentConfiguration : IAssessmentConfiguration
    {
        /// <summary>
        /// Externally provided Context: Inject all subsystems needed by this minigame
        /// </summary>
        public IGameContext Context { get; set; }

        /// <summary>
        /// Configured externally: which assessment we need to start.
        /// </summary>
        public AssessmentCode assessmentType = AssessmentCode.Unsetted;

        /// <summary>
        /// Externally provided Question provider
        /// </summary>
        private IQuestionProvider questionProvider;
        public IQuestionProvider Questions
        {
            get
            {
                return GetQuestionProvider();
            }
            set
            {
                questionProvider = value;
            }
        }

        private IQuestionProvider GetQuestionProvider()
        {
            return questionProvider;
        }

        /// <summary>
        /// Setted externally: assessments will scale quantity of content (number of questions
        /// and answers mostly) linearly with this value. It is assumed that Difficulty
        /// start with 0 and increase up to 1 as long as the Child progress in the world.
        /// The difficulty should be different for each assessmentType.
        /// </summary>
        public float Difficulty { get; set; }

        /// <summary>
        /// How many questions showed simultaneously on the screen.
        /// </summary>
        public int SimultaneosQuestions { get; private set; }

        /// <summary>
        /// How many answers should each question have. In Categorize assessments
        /// (The ones where the child should put something in the right category,
        /// like Sun/Moon) this is used to show maximum number of answers even when
        /// each question has a different number of answers (there could be 2 words
        /// to be putted in Moon, and 3 in Sun, in this case 3 placeholders are
        /// showed anyway).
        /// </summary>
        public int Answers { get; private set; } // number of answers in category questions

        /// <summary>
        /// Number of rounds, mostly fixed for each game, this value is setted
        /// inside SetupBuilder.
        /// </summary>
        public int Rounds { get { return _rounds; } private set { _rounds = value; } }
        private int _rounds = 0;

        /////////////////
        // Singleton Pattern
        static AssessmentConfiguration instance;
        public static AssessmentConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new AssessmentConfiguration();
                return instance;
            }
        }
        /////////////////


        /// <summary>
        /// This is the "Linear Scaler". It takes a range of parameters and output the value
        /// "in between" according to the Difficulty.
        /// </summary>
        private DifficultyRegulation snag;

        /// <summary>
        /// This is called by MiniGameAPI to create QuestionProvider, that means that if I start game
        /// from debug scene, I need a custom test Provider.
        /// </summary>
        /// <returns>Custom question data for the assessment</returns>
        public IQuestionBuilder SetupBuilder()
        {
            // Testing question builders
            snag = new DifficultyRegulation( Difficulty);

            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_Builder();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_Builder();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_Builder();

                case AssessmentCode.SunMoonWord:
                    return Setup_SunMoonWords_Builder();

                case AssessmentCode.SunMoonLetter:
                    return Setup_SunMoonLetter_Builder();

                case AssessmentCode.QuestionAndReply:
                    return Setup_QuestionAnReply_Builder();

                case AssessmentCode.SelectPronouncedWord:
                    return Setup_SelectPronuncedWord_Builder();

                case AssessmentCode.SingularDualPlural:
                    return Setup_SingularDualPlural_Builder();

                case AssessmentCode.WordArticle:
                    return Setup_WordArticle_Builder();

                case AssessmentCode.MatchWordToImage:
                    return Setup_MatchWordToImage_Builder();

                case AssessmentCode.CompleteWord:
                    return Setup_CompleteWord_Builder();

                case AssessmentCode.OrderLettersOfWord:
                    return Setup_OrderLettersOfWord_Builder();

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
        }

        private IQuestionBuilder Setup_OrderLettersOfWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.wordFilters.requireDrawings = true;
            
            return new LettersInWordQuestionBuilder(
                Rounds,
                2,
                useAllCorrectLetters: true,
                parameters: builderParams
                );
        }

        private IQuestionBuilder Setup_CompleteWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = true;
            builderParams.wordFilters.requireDrawings = true;

            return new LettersInWordQuestionBuilder(

                SimultaneosQuestions * Rounds,  // Total Answers
                1,                              // Always one!
                snag.Increase(3, 5),            // WrongAnswers
                useAllCorrectLetters: false,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_MatchWordToImage_Builder()
        {
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;
            builderParams.wordFilters.requireDrawings = true;
            SimultaneosQuestions = 1;
            Rounds = 3;
            int nCorrect = 1;
            int nWrong = snag.Increase( 2, 4);

            return new RandomWordsQuestionBuilder(
                SimultaneosQuestions * Rounds,
                nCorrect,
                nWrong,
                firstCorrectIsQuestion: true,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_WordArticle_Builder()
        {
            SimultaneosQuestions = 2;
            Rounds = 3;
            Answers = 2;
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wordFilters.excludeArticles = false;

            return new WordsByArticleQuestionBuilder(
                Answers * Rounds * 3,
                builderParams);
        }

        private IQuestionBuilder Setup_SingularDualPlural_Builder()
        {
            SimultaneosQuestions = 3;
            Rounds = 3;
            Answers = 2;
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wordFilters.excludePluralDual = false;

            return new WordsByFormQuestionBuilder(
                SimultaneosQuestions*Rounds*4,
                builderParams);
        }

        private IQuestionBuilder Setup_SelectPronuncedWord_Builder()
        {
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;
            SimultaneosQuestions = 1;
            Rounds = 3;
            int nCorrect = 1;
            int nWrong = snag.Increase(2, 4);
            return new RandomWordsQuestionBuilder(
                SimultaneosQuestions*Rounds,
                nCorrect,
                nWrong,
                firstCorrectIsQuestion: true,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_QuestionAnReply_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;
            int nWrongs = snag.Increase( 2, 4);

            return new  PhraseQuestionsQuestionBuilder(
                        SimultaneosQuestions * Rounds, // totale questions
                        nWrongs     // wrong additional answers
                );
        }

        private IQuestionBuilder Setup_SunMoonLetter_Builder()
        {
            SimultaneosQuestions = 2;
            Rounds = 3;
            Answers = 2;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;

            return new LettersBySunMoonQuestionBuilder( 
                        SimultaneosQuestions * Rounds * 2,
                        builderParams
            );
        }

        private IQuestionBuilder Setup_WordsWithLetter_Builder()
        {
            SimultaneosQuestions = 2;
            snag.SetStartingFrom( 0.5f);
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;

            return new WordsWithLetterQuestionBuilder( 

                SimultaneosQuestions*Rounds,    // Total Answers
                1,                              // Correct Answers
                snag.Increase( 1, 2),           // Wrong Answers
                parameters: builderParams
                );     

        }

        private IQuestionBuilder Setup_SunMoonWords_Builder()
        {
            SimultaneosQuestions = 2;
            Rounds = 3;
            Answers = 2;

            return new WordsBySunMoonQuestionBuilder( SimultaneosQuestions * Rounds * 2);
        }

        private IQuestionBuilder Setup_MatchLettersToWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;

            return new LettersInWordQuestionBuilder(

                SimultaneosQuestions * Rounds,   // Total Answers
                snag.Increase( 1, 2),            // CorrectAnswers
                snag.Increase( 2, 4),            // WrongAnswers
                useAllCorrectLetters: false,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_LetterShape_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;

            return new RandomLettersQuestionBuilder(
                SimultaneosQuestions * Rounds,  // Total Answers
                1,                              // CorrectAnswers
                snag.Increase( 3, 6),           // WrongAnswers
                firstCorrectIsQuestion:true,
                parameters:builderParams);
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_LearnRules();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_LearnRules();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_LearnRules();

                case AssessmentCode.SunMoonWord:
                    return Setup_SunMoonWords_LearnRules();

                case AssessmentCode.SunMoonLetter:
                    return Setup_SunMoonLetter_LearnRules();

                case AssessmentCode.QuestionAndReply:
                    return Setup_QuestionAnReply_LearnRules();

                case AssessmentCode.SelectPronouncedWord:
                    return Setup_SelectPronuncedWord_LearnRules();

                case AssessmentCode.SingularDualPlural:
                    return Setup_SingularDualPlural_LearnRules();

                case AssessmentCode.WordArticle:
                    return Setup_WordArticle_LearnRules();

                case AssessmentCode.MatchWordToImage:
                    return Setup_MatchWordToImage_LearnRules();

                case AssessmentCode.CompleteWord:
                    return Setup_CompleteWord_LearnRules();

                case AssessmentCode.OrderLettersOfWord:
                    return Setup_OrderLettersOfWord_LearnRules();

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
        }

        private MiniGameLearnRules Setup_OrderLettersOfWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_CompleteWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_MatchWordToImage_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_WordArticle_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SingularDualPlural_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SelectPronuncedWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_QuestionAnReply_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SunMoonLetter_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SunMoonWords_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_WordsWithLetter_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_MatchLettersToWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_LetterShape_LearnRules()
        {
            return new MiniGameLearnRules();
        }

    }
}
