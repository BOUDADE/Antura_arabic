﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class GameResultUI : MonoBehaviour
    {
        public EndgameResultPanel EndgameResultPanel;
        public EndsessionResultPanel EndsessionResultPanel;
        public GameObject[] RewardsGos;

        static GameResultUI I;
        const string ResourcesPath = "Prefabs/UI/GameResultUI";

        #region Unity + Init

        static void Init()
        {
            if (I != null) return;

            I = Instantiate(Resources.Load<GameResultUI>(ResourcesPath));
        }

        void Awake()
        {
            I = this;
        }

        void OnDestroy()
        {
            if (I == this) I = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void ShowEndgameResult(int _numStars)
        {
            Init();
            I.EndgameResultPanel.Show(_numStars);
        }

        /// <summary>
        /// Returns an array with 2 gameObjects inside which to place the rewards prefab (0 is first reward, 1 is second).
        /// <para><code>IMPORTANT:</code> do not modify the returned array, just use its data</para>
        /// </summary>
        /// <param name="_sessionData">A list of data for each minigame that was part of the session</param>
        /// <param name="_totUnlockedRewards">Total rewards unlocked in previous sessions (0 to 2)</param>
        /// <param name="_immediate">If TRUE shows the screen immediately with no animations. You can ignore this</param>
        /// <returns></returns>
        public static GameObject[] ShowEndsessionResult(List<EndsessionResultData> _sessionData, int _totUnlockedRewards, bool _immediate = false)
        {
            Init();
            I.EndsessionResultPanel.Show(_sessionData, _immediate);
            return I.RewardsGos;
        }

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void HideEndgameResult(bool _immediate = false)
        {
            if (I == null) return;

            I.EndgameResultPanel.Hide(_immediate);
        }

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void HideEndsessionResult(bool _immediate = false)
        {
            if (I == null) return;

            I.EndsessionResultPanel.Hide(_immediate);
        }

        #endregion
    }
}