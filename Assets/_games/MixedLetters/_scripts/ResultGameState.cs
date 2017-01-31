﻿using EA4S.MinigamesCommon;
using EA4S.UI;

namespace EA4S.Minigames.MixedLetters
{
    public class ResultGameState : IState
    {
        private MixedLettersGame game;

        private const float TWIRL_ANIMATION_BACK_SHOWN_DELAY = 1f;
        private const float END_RESULT_DELAY = 1f;

        private float twirlAnimationDelayTimer;
        private bool wasBackShownDuringTwirlAnimation;
        private float endResultTimer;
        private bool isGameOver;

        private int lastNumStarsWentTo = 0;

        public ResultGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            SeparateLettersSpawnerController.instance.SetLettersNonInteractive();

            game.DisableRepeatPromptButton();

            if (game.roundNumber != 0)
            {
                MinigamesUI.Timer.Pause();
            }

            if (!game.WasLastRoundWon)
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                SeparateLettersSpawnerController.instance.ShowLoseAnimation(OnResultAnimationEnded);
            }

            else
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Win);
                SeparateLettersSpawnerController.instance.ShowWinAnimation(OnVictimLLIsShowingBack, OnResultAnimationEnded);

                int numStarsAsOfCurrentRound = game.GetNumStarsAsOfCurrentRound();

                if (numStarsAsOfCurrentRound != lastNumStarsWentTo)
                {
                    MinigamesUI.Starbar.GotoStar(numStarsAsOfCurrentRound - 1);
                    lastNumStarsWentTo = numStarsAsOfCurrentRound;
                }
            }

            twirlAnimationDelayTimer = TWIRL_ANIMATION_BACK_SHOWN_DELAY;
            wasBackShownDuringTwirlAnimation = false;
            endResultTimer = END_RESULT_DELAY;
            isGameOver = false;

            // Increase the round number here so the victim LL loads the prompt of the next round correctly during
            // the twirl animation:
            game.roundNumber++;
        }

        private void OnVictimLLIsShowingBack()
        {
            if (!game.IsGameOver)
            {
                game.GenerateNewWord();
                VictimLLController.instance.HideVictoryRays();
            }
            
            wasBackShownDuringTwirlAnimation = true;
        }

        public void ExitState()
        {
            game.ResetScene();
        }

        public void OnResultAnimationEnded()
        {
            if (!game.IsGameOver)
            {
                game.SetCurrentState(game.IntroductionState);
            }

            else
            {
                isGameOver = true;

                if (game.WasLastRoundWon)
                {
                    endResultTimer = 0f;
                }
            }
        }

        public void Update(float delta)
        {
            if (isGameOver)
            {
                endResultTimer -= delta;

                if (endResultTimer < 0)
                {
                    game.EndGame(game.GetNumStarsAsOfCurrentRound(), 0);
                }
            }

            else if (game.WasLastRoundWon)
            {
                if (wasBackShownDuringTwirlAnimation)
                {
                    twirlAnimationDelayTimer -= delta;

                    if (twirlAnimationDelayTimer <= 0)
                    {
                        OnResultAnimationEnded();
                    }
                }
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
