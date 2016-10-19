﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/28 15:04
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Global UI created dynamically at runtime,
    /// contains all global UI elements > Pause, SceneTransitioner, ContinueScreen, PopupScreen
    /// </summary>
    [ScriptExecutionOrder(-100)]
    public class GlobalUI : MonoBehaviour
    {
        public static GlobalUI I { get; private set; }
        public static SceneTransitioner SceneTransitioner { get; private set; }
        public static ContinueScreen ContinueScreen { get; private set; }
        public static WidgetPopupWindow WidgetPopupWindow { get; private set; }
        public static WidgetSubtitles WidgetSubtitles { get; private set; }
        public static PauseMenu PauseMenu { get; private set; }

        public ActionFeedbackComponent ActionFeedback { get; private set; }

        const string ResourceId = "Prefabs/UI/GlobalUI";

        public static void Init()
        {
            if (I != null)
                return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourceId));
            go.name = "[GlobalUI]";
            DontDestroyOnLoad(go);
        }

        void Awake()
        {
            I = this;

            // Awake all global UI elements
            SceneTransitioner = StoreAndAwake<SceneTransitioner>();
            ContinueScreen = StoreAndAwake<ContinueScreen>();
            WidgetPopupWindow = StoreAndAwake<WidgetPopupWindow>();
            WidgetSubtitles = StoreAndAwake<WidgetSubtitles>();
            PauseMenu = StoreAndAwake<PauseMenu>();
            ActionFeedback = StoreAndAwake<ActionFeedbackComponent>();
        }

        void OnDestroy()
        {
            I = null;
        }

        /// <summary>
        /// Immediately clears the GlobalUI elements
        /// </summary>
        /// <param name="includeSceneTransitioner">If TRUE (default) also clears the sceneTransitioner, otherwise not</param>
        public static void Clear(bool includeSceneTransitioner = true)
        {
            if (includeSceneTransitioner && SceneTransitioner != null)
                SceneTransitioner.CloseImmediate();
            if (ContinueScreen != null)
                ContinueScreen.Close(true);
            if (WidgetPopupWindow != null)
                WidgetPopupWindow.Close(true);
            if (WidgetSubtitles != null)
                WidgetSubtitles.Close(true);
        }

        public static void ShowPauseMenu(bool visible)
        {
            PauseMenu.gameObject.SetActive(visible);
        }


        T StoreAndAwake<T>() where T : Component
        {
            T obj = this.GetComponentInChildren<T>(true);
            obj.gameObject.SetActive(true);
            return obj;
        }
    }
}