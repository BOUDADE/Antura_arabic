﻿using UnityEngine;
using EA4S.MinigamesCommon;

namespace EA4S.Minigames.ThrowBalls
{
    public class ThrowBallsGame : MiniGame
    {
        public GameState GameState { get; private set; }

        public static ThrowBallsGame instance;

        public enum ThrowBallsDifficulty
        {
            VeryEasy, Easy, Normal, Hard, VeryHard
        }
        private ThrowBallsDifficulty _difficulty;
        public ThrowBallsDifficulty Difficulty
        {
            get
            {
                return _difficulty;
            }
        }

        public GameObject ball;
        public BallController ballController;

        public GameObject letterWithPropsPrefab;

        public GameObject poofPrefab;
        public GameObject cratePoofPrefab;

        public GameObject environment;

        protected override void OnInitialize(IGameContext context)
        {
            instance = this;

            SetDifficulty();

            GameState = new GameState(this);
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return ThrowBallsConfiguration.Instance;
        }

        protected override IState GetInitialState()
        {
            return GameState;
        }

        private void SetDifficulty()
        {
            float difficultyAsAFloat = ThrowBallsConfiguration.Instance.Difficulty;

            if (difficultyAsAFloat < 0.2f)
            {
                _difficulty = ThrowBallsDifficulty.VeryEasy;
            }

            else if (difficultyAsAFloat < 0.4f)
            {
                _difficulty = ThrowBallsDifficulty.Easy;
            }

            else if (difficultyAsAFloat < 0.6f)
            {
                _difficulty = ThrowBallsDifficulty.Normal;
            }

            else if (difficultyAsAFloat < 0.8f)
            {
                _difficulty = ThrowBallsDifficulty.Hard;
            }

            else
            {
                _difficulty = ThrowBallsDifficulty.VeryHard;
            }
        }
    }
}