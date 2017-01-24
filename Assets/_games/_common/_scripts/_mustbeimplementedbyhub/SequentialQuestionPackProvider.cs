﻿using System.Collections.Generic;

namespace EA4S.MinigamesAPI
{

    /// <summary>
    /// Default IQuestionProvider that returns packs in a sequential order.
    /// </summary>
    /// <seealso cref="IQuestionProvider" />
    // refactor: this is used in all minigames as the core application reasons only in terms of question packs
    public class SequentialQuestionPackProvider : IQuestionProvider {

        #region properties
        List<IQuestionPack> questions = new List<IQuestionPack>();
        int currentQuestion;
        #endregion

        public SequentialQuestionPackProvider(List<IQuestionPack> _questionsPack) {
            currentQuestion = 0;

            questions.AddRange(_questionsPack);
        }

        /// <summary>
        /// Provide me another question.
        /// </summary>
        /// <returns></returns>
        IQuestionPack IQuestionProvider.GetNextQuestion() {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }

    }
}