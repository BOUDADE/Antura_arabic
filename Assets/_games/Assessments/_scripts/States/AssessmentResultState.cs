using Kore.Coroutines;
using System.Collections;

namespace EA4S.Assessment
{
    /// <summary>
    /// Result state. notify the LogManager of game ended and play final animation.
    /// Also teleport to main map.
    /// </summary>
    public class AssessmentResultState : IState
    {
        private AssessmentGame assessmentGame;
        private AssessmentDialogues dialogueManager;

        public AssessmentResultState( AssessmentGame assessmentGame, AssessmentDialogues dialogueManager)
        {
            this.assessmentGame = assessmentGame;
            this.dialogueManager = dialogueManager;
        }

        public void EnterState()
        {
            AssessmentConfiguration.Instance.Context.GetLogManager().OnGameEnded(3);

            var audioManager = assessmentGame.Context.GetAudioManager();

            audioManager.PlayMusic( Music.Relax);
            audioManager.PlaySound( Sfx.TickAndWin);
            dialogueManager.PlayAssessmentCompleteSound();

            Koroutine.Run( QuitAfterSomeTime( seconds: 1));
        }

        IEnumerator QuitAfterSomeTime(float seconds)
        {
            yield return Wait.For( seconds);
            ExitState();
        }

        bool exited = false;
        public void ExitState()
        {
            if (exited == false) {
                AppManager.I.NavigationManager.GoToNextScene();// AppScene.Rewards
                exited = true;
            }
        }

        public void Update( float delta)
        {

        }

        public void UpdatePhysics( float delta)
        {

        }
    }
}
