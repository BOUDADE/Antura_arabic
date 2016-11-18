﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    /// <summary>
    /// Star (bone) and its bg are in separate gameObjects (to use correct overlay)
    /// </summary>
    public class EndgameStar : MonoBehaviour
    {
        public Color ungainedColor = Color.red;
        [Header("References")]
        public GameObject Bg;
        public Image BgRays, BgCircle;

        public RectTransform RectT { get; private set; }
        bool setupDone;
        Tween gainTween;

        #region Unity + Setup

        void Setup()
        {
            if (setupDone) return;

            setupDone = true;
            RectT = this.GetComponent<RectTransform>();

            gainTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(BgRays.transform.DOScale(0.001f, 0.3f).From())
                .Join(BgCircle.DOFade(0, 0.3f).From())
                .Append(BgRays.transform.DORotate(new Vector3(0, 0, -360), 9f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(999))
                .Insert(0, this.GetComponent<Image>().DOColor(ungainedColor, 0.3f).From())
                .Join(this.transform.DOScale(0.5f, 0.0001f).From())
                .Join(this.transform.DOPunchRotation(new Vector3(0, 0, 36), 0.5f))
                .Insert(0.0001f, this.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f));
        }

        void Awake()
        {
            Setup();
        }

        void OnDestroy()
        {
            gainTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            if (!setupDone) return;

            gainTween.Rewind();
        }

        public void Gain()
        {
            Setup();
            gainTween.PlayForward();
        }

        #endregion
    }
}