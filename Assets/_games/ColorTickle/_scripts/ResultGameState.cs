﻿using UnityEngine;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class ResultGameState : IGameState
    {
        ColorTickleGame game;

        float timer = 0.5f;
        public ResultGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.endCanvas.gameObject.SetActive(true);
            Debug.Log("Result State activated");
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.EndGame(game.m_Stars,0);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
