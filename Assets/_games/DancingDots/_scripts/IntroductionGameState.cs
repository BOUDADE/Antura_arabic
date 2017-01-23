﻿using EA4S.Audio;
using EA4S.MinigamesCommon;
using UnityEngine;

namespace EA4S.Minigames.DancingDots
{
    public class IntroductionGameState : IGameState
    {
        DancingDotsGame game;

        float timer = 1.5f;
        public IntroductionGameState(DancingDotsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            this.game.dancingDotsLL.contentGO.SetActive(false);
            Debug.Log("Intro");
            AudioManager.I.PlayDialogue("DancingDots_Title");
            game.dancingDotsLL.letterObjectView.DoTwirl(null);
            game.disableInput = true;
            //game.StartRound();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.QuestionState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}