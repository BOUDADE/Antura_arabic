﻿using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using System.Linq;
using System;
using EA4S.Db;
using EA4S.Log;

namespace EA4S.API {

    /// <summary>
    /// Entry point to start fame from app.
    /// </summary>
    /// <seealso cref="ModularFramework.Core.Singleton{EA4S.API.MiniGameAPI}" />
    public class MiniGameAPI : Singleton<MiniGameAPI> {

        #region Gameplay Management

        /// <summary>
        /// Prepare all context needed and starts the game.
        /// </summary>
        /// <param name="_gameCode">The game code.</param>
        /// <param name="_gameConfiguration">The game configuration.</param>
        public void StartGame(MiniGameCode _gameCode, GameConfiguration _gameConfiguration) {
            // To be deleted
            List<IQuestionPack> _gameData = null;

            MiniGameData miniGameData = AppManager.Instance.DB.GetMiniGameDataByCode(_gameCode);
            string miniGameScene = miniGameData.Scene;
            IQuestionBuilder rules = null;
            IGameConfiguration actualConfig = null;

            switch (_gameCode) {
                case MiniGameCode.Assessment_LetterShape:
                case MiniGameCode.Assessment_WordsWithLetter:
                case MiniGameCode.Assessment_LetterInWord:
                    break;
                case MiniGameCode.AlphabetSong:
                    // Must be defined how use sentence data structure
                    break;
                case MiniGameCode.Balloons_counting:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Counting;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_letter:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Letter;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_spelling:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Spelling;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_words:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Words;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.ColorTickle:
                    ColorTickle.ColorTickleConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ColorTickle.ColorTickleConfiguration.Instance;
                    break;
                case MiniGameCode.DancingDots:
                    DancingDots.DancingDotsConfiguration.Instance.Variation = DancingDots.DancingDotsVariation.V_1;
                    DancingDots.DancingDotsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = DancingDots.DancingDotsConfiguration.Instance;
                    break;
                case MiniGameCode.DontWakeUp:
                    // 
                    break;
                case MiniGameCode.Egg:
                    Egg.EggConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Egg.EggConfiguration.Instance;
                    miniGameScene = "game_Egg"; // TODO: check
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Alphabet;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_counting:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Counting;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_letter:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Letter;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Spelling;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_words:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Words;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.TakeMeHome:
                    TakeMeHome.TakeMeHomeConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = TakeMeHome.TakeMeHomeConfiguration.Instance;
                    break;
                case MiniGameCode.HiddenSource:
                    // It has now become TakeMeHome
                    break;
                case MiniGameCode.HideSeek:
                    HideAndSeek.HideAndSeekConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = HideAndSeek.HideAndSeekConfiguration.Instance;
                    break;
                case MiniGameCode.MakeFriends:
                    MakeFriends.MakeFriendsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MakeFriends.MakeFriendsConfiguration.Instance;
                    break;
                case MiniGameCode.Maze:
                    Maze.MazeConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Maze.MazeConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter:
                    MissingLetter.MissingLetterConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    // Must be defined how use sentence data structure
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    // TODO: set variation
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Alphabet;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Spelling;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.SickLetters:
                    SickLetters.SickLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = SickLetters.SickLettersConfiguration.Instance;
                    break;
                case MiniGameCode.ReadingGame:
                    // Must be defined how use sentence data structure
                    break;
                case MiniGameCode.Scanner:
//                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.V_1;
                    Scanner.ScannerConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Scanner.ScannerConfiguration.Instance;
                    break;
//                case MiniGameCode.Scanner_phrase:
//                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.phrase;
//                    Scanner.ScannerConfiguration.Instance.Context = AnturaMinigameContext.Default;
//                    actualConfig = Scanner.ScannerConfiguration.Instance;
//                    break;
                case MiniGameCode.ThrowBalls_letters:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.letters;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_words:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.words;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letterinword:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.lettersinword;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_letters:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.LetterInAWord;
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_words:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.SunMoon;
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                default:
                    Debug.LogWarningFormat("Minigame selected {0} not found.", _gameCode.ToString());
                    break;
            }

            // Set game configuration instance with game data
            // game difficulty
            actualConfig.Difficulty = _gameConfiguration.Difficulty;
            // rule setted in config and used by AI to create correct game data
            rules = actualConfig.SetupBuilder();
            // question packs (game data)
            actualConfig.Questions = new FindRightLetterQuestionProvider(AppManager.Instance.GameLauncher.RetrieveQuestionPacks(rules), miniGameData.Description);

            // Save current game code to appmanager currentminigame
            AppManager.Instance.CurrentMinigame = miniGameData;

            // Comunicate to LogManager that start new single minigame play session.
            actualConfig.Context.GetLogManager().InitGameplayLogSession(_gameCode);

            // Call game start
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(miniGameScene);
        }

        #endregion

    }
}