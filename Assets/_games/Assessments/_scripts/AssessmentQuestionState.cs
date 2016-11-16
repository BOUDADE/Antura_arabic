using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentQuestionState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentQuestionState( AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        public void EnterState()
        {
            // Enable popup widget
            var popupWidget = assessmentGame.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback( OnPopupCloseRequested);
            popupWidget.SetMessage( AssessmentConfiguration.Instance.Description, true);
        }

        void OnPopupCloseRequested()
        {
            assessmentGame.SetCurrentState( assessmentGame.PlayState);
        }

        public void ExitState()
        {
            assessmentGame.Context.GetPopupWidget().Hide();
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}
