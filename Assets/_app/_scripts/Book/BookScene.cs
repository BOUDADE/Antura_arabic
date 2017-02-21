﻿using EA4S.Audio;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.PlayerBook
{
    public enum BookArea
    {
        None,
        Vocabulary,
        Player,
        Journey,
        MiniGames
    }

    /// <summary>
    /// Manages the Player Book scene.
    /// - shows unlocked learning content
    /// - provides information on player progression
    /// - grants direct access to minigames
    /// - grants access to the Parents' Panel
    /// </summary>
    public class BookScene : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;
        public BookArea OpeningArea;

        [Header("References")]
        public GameObject BookPanel;
        public GameObject PlayerPanel;
        public GameObject JourneyPanel;
        public GameObject GamesPanel;

        public UIButton BtnBook;
        public UIButton BtnPlayer;
        public UIButton BtnJourney;
        public UIButton BtnGames;

        BookArea currentPanel = BookArea.None;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true, GoBackCustom);
            AudioManager.I.PlayMusic(SceneMusic);
            LogManager.I.LogInfo(InfoEvent.Book, "enter");

            SceneTransitioner.Close();

            AudioManager.I.PlayDialogue("Book_Intro");

            HideAllPanels();
            OpenArea(OpeningArea);
        }

        void OpenArea(BookArea newPanel)
        {
            if (newPanel != currentPanel) {
                activatePanel(currentPanel, false);
                currentPanel = newPanel;
                activatePanel(currentPanel, true);
                ResetMenuButtons();
            }
        }

        void activatePanel(BookArea panel, bool status)
        {
            switch (panel) {
                case BookArea.Vocabulary:
                    BookPanel.SetActive(status);
                    break;
                case BookArea.Journey:
                    JourneyPanel.SetActive(status);
                    break;
                case BookArea.Player:
                    PlayerPanel.SetActive(status);
                    break;
                case BookArea.MiniGames:
                    GamesPanel.SetActive(status);
                    break;
            }
        }

        void HideAllPanels()
        {
            BookPanel.SetActive(false);
            PlayerPanel.SetActive(false);
            JourneyPanel.SetActive(false);
            GamesPanel.SetActive(false);
        }

        void ResetMenuButtons()
        {
            BtnBook.Lock(currentPanel == BookArea.Vocabulary);
            BtnPlayer.Lock(currentPanel == BookArea.Player);
            BtnJourney.Lock(currentPanel == BookArea.Journey);
            BtnGames.Lock(currentPanel == BookArea.MiniGames);
        }

        public void BtnOpenBook()
        {
            OpenArea(BookArea.Vocabulary);
        }

        public void BtnOpenPlayer()
        {
            OpenArea(BookArea.Player);
        }

        public void BtnOpenJourney()
        {
            OpenArea(BookArea.Journey);
        }

        public void BtnOpenGames()
        {
            OpenArea(BookArea.MiniGames);
        }

        public void GoBackCustom()
        {
            LogManager.I.LogInfo(InfoEvent.Book, "exit");
            AppManager.I.NavigationManager.GoBack();
        }
    }
}