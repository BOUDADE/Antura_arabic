using DG.Tweening;
using EA4S.Helpers;
using Kore.Coroutines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Place answers to random places over a grid with small displacement
    /// (answers are not overlapped and start in different positions,
    /// without overlapping the question area).
    /// </summary>
    public class OrderedAnswerPlacer : IAnswerPlacer
    {
        public OrderedAnswerPlacer( AssessmentAudioManager audioManager, QuestionPlacerOptions placerOptions)
        {
            this.audioManager = audioManager;
            this.placerOptions = placerOptions;
        }

        private bool isAnimating = false;
        public bool IsAnimating()
        {
            return isAnimating;
        }

        private Answer[] allAnswers;
        private AssessmentAudioManager audioManager;
        private QuestionPlacerOptions placerOptions;

        public void Place(Answer[] answer)
        {
            allAnswers = answer;
            isAnimating = true;
            Koroutine.Run( PlaceCoroutine());
        }

        public void RemoveAnswers()
        {
            isAnimating = true;
            Koroutine.Run( RemoveCoroutine());
        }

        private IEnumerator PlaceCoroutine()
        {
            List< Vector3> positions = new List< Vector3>();
            float xMin = placerOptions.LeftX + placerOptions.AnswerSize/2f + 2.0f;
            float xMax = placerOptions.RightX - placerOptions.AnswerSize/2f - 1.0f;
            float yMin = placerOptions.BottomY + 2.3f;
            float z = 5f;

            float deltaX = placerOptions.RightX - placerOptions.LeftX;
            float occupiedSpace = 0;

            int i = 0;
            foreach (var a in allAnswers)
                i++;



            for (float x = xMin; x < xMax; x += placerOptions.AnswerSize + 0.2f)
            {
                int times = 0;
                for (float y = yMin; times < 3; y += 3.1f, times++)
                {
                    float dx = Random.Range( -0.1f, 0.1f);
                    var vec = new Vector3( x + dx, y, z);
                    positions.Add( vec);
                }
            }

            positions.Shuffle();

            foreach (var a in allAnswers)
                yield return Koroutine.Nested( PlaceAnswer( a, positions));

            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        private IEnumerator PlaceAnswer( Answer answer, List< Vector3> positions)
        {
            var go = answer.gameObject;
            go.transform.localPosition = positions.Pull();
            go.GetComponent< StillLetterBox>().InstaShrink();
            go.GetComponent< StillLetterBox>().Poof();
            go.GetComponent< StillLetterBox>().Magnify();
            audioManager.PlayPoofSound();

            yield return Wait.For( Random.Range( 0.07f, 0.13f));
        }

        private IEnumerator RemoveCoroutine()
        {
            foreach (var a in allAnswers)
                yield return Koroutine.Nested( RemoveAnswer( a.gameObject));

            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        private IEnumerator RemoveAnswer( GameObject answ)
        {
            audioManager.PlayPoofSound();

            answ.GetComponent< StillLetterBox>().Poof();
            answ.transform.DOScale( 0, 0.3f).OnComplete( () => GameObject.Destroy( answ));

            yield return Wait.For( 0.1f);
        }
    }
}
