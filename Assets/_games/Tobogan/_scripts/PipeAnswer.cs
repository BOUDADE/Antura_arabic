﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using ArabicSupport;
using System.Collections.Generic;

namespace EA4S.Tobogan
{
    public class PipeAnswer : MonoBehaviour
    {
        public TMP_Text answerText;
        public Image answerImage;

        public GameObject aspirationParticle;
        public GameObject graphics;
        public TremblingTube trembling;

        public Transform signTransform;

        public bool IsCorrectAnswer { get; private set; }

        public event Action<PipeAnswer> onTriggerEnterPipe;
        public event Action<PipeAnswer> onTriggerExitPipe;

        List<Material> tubeMaterials = new List<Material>();

        public bool active;

        bool showSign = true;

        float easeTimer;
        const float EASE_DURATION = 4.0f;
        public bool ShowSign
        {
            set
            {
                if (value == showSign)
                    return;

                easeTimer = EASE_DURATION;
                showSign = value;
            }
        }

        const float DISAPPEAR_HEIGHT = 6.5f;
        float disappearSpeed;

        public AnimationCurve easeCurve;

        void Start()
        {
            StopSelectedAnimation();

            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Clear();
            }

            foreach (var renderer in graphics.GetComponentsInChildren<MeshRenderer>(true))
                tubeMaterials.Add(renderer.material);

            aspirationParticle.SetActive(true);
            graphics.transform.localPosition = Vector3.up * DISAPPEAR_HEIGHT;
            disappearSpeed = 4.0f + 2 * UnityEngine.Random.value;
        }

        public void Update()
        {
            Vector3 targetPosition = Vector3.zero;

            if (!active)
                targetPosition = Vector3.up * DISAPPEAR_HEIGHT;

            graphics.transform.localPosition = Vector3.Lerp(graphics.transform.localPosition, targetPosition, disappearSpeed * Time.deltaTime);
            
          
            if (showSign)
            {
                answerText.alpha = Mathf.Lerp(answerText.alpha, 1, Time.deltaTime * 5.0f);
                signTransform.localRotation = Quaternion.Slerp(signTransform.localRotation, Quaternion.identity, Time.deltaTime * 5.0f);
            }
            else
            {
                easeTimer -= Time.deltaTime;
                if (easeTimer < 0)
                    easeTimer = 0;

                float t = easeCurve.Evaluate(1 - (easeTimer / EASE_DURATION));
                answerText.alpha = 1 - t;

                signTransform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0, 90, 0), t);
            }
        }

        public void SetAnswer(ILivingLetterData livingLetterData, bool correct)
        {
            if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                answerText.gameObject.SetActive(true);
                //answerImage.gameObject.SetActive(false);

                answerText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LL_LetterData)livingLetterData).Data.Isolated_Unicode);
            }
            else if (livingLetterData.DataType == LivingLetterDataType.Word)
            {
                answerText.gameObject.SetActive(true);
                //answerImage.gameObject.SetActive(true);

                answerText.text = ArabicFixer.Fix(((LL_WordData)livingLetterData).Data.Arabic, false, false);
                //answerImage.sprite = livingLetterData.DrawForLivingLetter;
            }
            //else
            //{
            //    answerImage.gameObject.SetActive(true);
            //    answerImage.sprite = livingLetterData.DrawForLivingLetter;

            //    answerText.gameObject.SetActive(false);
            //}
            IsCorrectAnswer = correct;
        }

        void OnTriggerEnter(Collider other)
        {
            if (onTriggerEnterPipe != null)
            {
                onTriggerEnterPipe(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (onTriggerExitPipe != null)
            {
                onTriggerExitPipe(this);
            }
        }

        public void EnterAnimation()
        {

        }

        public void ExitAnimation()
        {

        }

        public void PlaySelectedAnimation()
        {
            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Play();
            }

            trembling.Trembling = true;

            for (int i = 0, count = tubeMaterials.Count; i < count; ++i)
                tubeMaterials[i].SetFloat("_OpeningAnimation", 1);
        }

        public void StopSelectedAnimation()
        {
            foreach (var particles in aspirationParticle.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Stop();
            }

            trembling.Trembling = false;

            for (int i = 0, count = tubeMaterials.Count; i < count; ++i)
                tubeMaterials[i].SetFloat("_OpeningAnimation", 0);
        }
    }
}