using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultAnswer : IAnswer
    {
        private LetterObjectView view;
        private bool isCorrect;

        public DefaultAnswer( LetterObjectView letter, bool correct)
        {
            view = letter;
            isCorrect = correct;
            var answer = letter.gameObject.AddComponent< AnswerBehaviour>();
            answer.SetAnswer( this);
        }

        public GameObject gameObject
        {
            get
            {
                return view.gameObject;
            }
        }

        public bool IsCorrect()
        {
            return isCorrect;
        }

        int answerSet = 0;

        public void SetAnswerSet( int set)
        {
            answerSet = set;
        }

        public int GetAnswerSet()
        {
            return answerSet;
        }
    }
}
