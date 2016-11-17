﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.MixedLetters
{
    public class DropZoneController : MonoBehaviour
    {
        public static DropZoneController chosenDropZone;

        public static float DropZoneZ = -41.5f;

        public SpriteRenderer spriteRenderer;
        
        private float THROB_INIT_SCALE;
        private float THROB_SCALE_MULTIPLIER = 1.2f;
        private float THROB_PERIOD = 0.33f;
        private float LETTER_SWAP_DROP_OFFSET = -1f;
        private IEnumerator throbAnimation;
        private bool isChosen = false;
        public SeparateLetterController droppedLetter;

        public RotateButtonController rotateButtonController;
        
        void Start()
        {
            THROB_INIT_SCALE = transform.localScale.x;

            Vector3 rotateButtonPosition = transform.position;
            rotateButtonPosition.y += 2.2f;
            rotateButtonPosition.z += 0.5f;
            rotateButtonController.SetPosition(rotateButtonPosition);
        }
        
        void FixedUpdate()
        {
            if (isChosen && chosenDropZone != this)
            {
                isChosen = false;
                Unhighlight();
            }
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void Highlight()
        {
            spriteRenderer.color = Color.yellow;
        }

        public void Unhighlight()
        {
            spriteRenderer.color = Color.white;
        }

        public void SetDroppedLetter(SeparateLetterController letter)
        {
            if (letter != null)
            {
                if (droppedLetter != null)
                {
                    Vector3 position = transform.position;
                    position.y += LETTER_SWAP_DROP_OFFSET;
                    droppedLetter.SetPosition(position, false);
                    droppedLetter.SetIsKinematic(false);
                    droppedLetter.droppedZone = null;
                }

                rotateButtonController.Enable();
            }

            else
            {
                rotateButtonController.Disable();
            }

            droppedLetter = letter;
            Unhighlight();
        }

        public void OnTriggerEnter(Collider collider)
        {
            //if (droppedLetter == null)
            //{
                Throb();
                isChosen = true;
                chosenDropZone = this;
                Highlight();
            //}
        }

        public void OnTriggerExit(Collider collider)
        {
            if (isChosen)
            {
                isChosen = false;
                chosenDropZone = null;
            }

            Unhighlight();
        }

        private void Throb()
        {
            if (throbAnimation != null)
            {
                StopCoroutine(throbAnimation);
            }

            throbAnimation = ThrobCoroutine();
            StartCoroutine(throbAnimation);
        }

        private IEnumerator ThrobCoroutine()
        {
            float throbFinalScale = THROB_INIT_SCALE * THROB_SCALE_MULTIPLIER;
            float throbScaleIncrementPerFixedFrame = ((throbFinalScale - THROB_INIT_SCALE) * Time.fixedDeltaTime) / (THROB_PERIOD * 0.5f);

            Vector3 scale = new Vector3(THROB_INIT_SCALE, THROB_INIT_SCALE, 1);

            transform.localScale = scale;

            while (true)
            {
                scale.x += throbScaleIncrementPerFixedFrame;
                scale.y += throbScaleIncrementPerFixedFrame;
                if (scale.x > throbFinalScale)
                {
                    throbScaleIncrementPerFixedFrame *= -1;
                }
                else if (scale.x < THROB_INIT_SCALE)
                {
                    transform.localScale = new Vector3(THROB_INIT_SCALE, THROB_INIT_SCALE, 1);
                    break;
                }
                transform.localScale = scale;
                yield return new WaitForFixedUpdate();
            }
        }

        public void OnRotateLetter()
        {
            if (droppedLetter != null)
            {
                droppedLetter.RotateCCW();
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.WheelTick);
                MixedLettersGame.instance.VerifyLetters();
            }
        }

        public void Reset()
        {
            droppedLetter = null;
            Unhighlight();
            isChosen = false;

            rotateButtonController.Disable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}