﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using EA4S;
using System.Collections.Generic;
using DG.Tweening;
using EA4S.Core;
using EA4S.Map;

namespace EA4S.Map
{
    public class LetterMovement : MonoBehaviour
    {
        [Header("Stage")]
        public Stage stageScript;

        [Header("UIButtons")]
        public GameObject moveRightButton;
        public GameObject moveLeftButton;

        float distanceNextDotToHitPoint;
        float distanceBeforelDotToHitPoint;
        float distanceActualDotToHitPoint;
        int dotCloser;
        Rope ropeSelected;
        Collider colliderRaycast;
        Tween moveTween, rotateTween;

        void Start()
        {
            Floating();

            /* FIRST CONTACT FEATURE */
            if (!AppManager.I.Player.IsFirstContact()) {
                AmIFirstorLastPos();
            }
            /* --------------------- */


        //    stageScript.positionPin++;
          //  MoveTo(stageScript.positionsPlayerPin[1].transform.position, true);
        }
        void Floating()
        {
            transform.DOBlendableMoveBy(new Vector3(0, 1, 0), 1).SetLoops(-1, LoopType.Yoyo);
        }

        void OnDestroy()
        {
            moveTween.Kill();
            rotateTween.Kill();
        }

        void FixedUpdate()
        {
           /* Debug.Log(AppManager.I.Player.CurrentJourneyPosition.Stage);
            Debug.Log(AppManager.I.Player.CurrentJourneyPosition.LearningBlock);
            Debug.Log(AppManager.I.Player.CurrentJourneyPosition.PlaySession);

            Debug.Log("Max"+AppManager.I.Player.MaxJourneyPosition.Stage);
            Debug.Log("MaxLB"+AppManager.I.Player.MaxJourneyPosition.LearningBlock);
            Debug.Log("MaxPS"+AppManager.I.Player.MaxJourneyPosition.PlaySession);  */   

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask)) {
                    if (hit.collider.tag == "Rope") {
                        ropeSelected = hit.transform.parent.gameObject.GetComponent<Rope>();
                        int numDotsRope = 0;
                        for (int r=0;r<ropeSelected.dots.Count;r++)
                        {
                            if (ropeSelected.dots[r].activeInHierarchy) numDotsRope++;
                        }
                        if (numDotsRope>1)
                        {
                            float distaceHitToDot = 1000;
                            float distanceHitBefore = 0;
                            dotCloser = 0;

                            for (int i = 0; i < numDotsRope; i++) {
                                distanceHitBefore = Vector3.Distance(hit.point,
                                    ropeSelected.dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot) {
                                    distaceHitToDot = distanceHitBefore;
                                    dotCloser = i;
                                }
                            }
                        } else {
                            dotCloser = 0;
                        }                     
                        colliderRaycast = hit.collider;
                        MoveToDot();
                    } else if (hit.collider.tag == "Pin") {
                        colliderRaycast = hit.collider;
                        MoveToPin();
                    } else colliderRaycast = null;
                } else colliderRaycast = null;
            } 
        }
        void LateUpdate()
        {
            if (Input.GetMouseButtonUp(0) && (!EventSystem.current.IsPointerOverGameObject()) && (colliderRaycast != null))
            {
                if (colliderRaycast.tag == "Rope") MoveToDot();
                else if (colliderRaycast.tag == "Pin") MoveToPin();
            }
        }

        void MoveToDot()
        {
            stageScript.positionPin = ropeSelected.dots[dotCloser].GetComponent<Dot>().pos;
            MoveTo(stageScript.positionsPlayerPin[stageScript.positionPin].transform.position);
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].GetComponent<Dot>().playSessionActual;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].GetComponent<Dot>().learningBlockActual;        
            UpdateCurrentJourneyPosition();
            AmIFirstorLastPos();
            transform.LookAt(stageScript.pines[ropeSelected.learningBlockRope].transform);
        }
        void MoveToPin()
        {
            MoveTo(colliderRaycast.transform.position);
            stageScript.positionPin = colliderRaycast.transform.gameObject.GetComponent<MapPin>().pos;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = colliderRaycast.transform.gameObject.GetComponent<MapPin>().learningBlockPin;
            if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < stageScript.numberLearningBlocks)
                transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1].transform);
            else transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform);
            UpdateCurrentJourneyPosition();
            AmIFirstorLastPos();
        }
        public void MoveToTheRightDot()
        {
            if((stageScript.positionPin < (stageScript.positionsPlayerPin.Count-1)) && (stageScript.positionPin!=stageScript.positionPinMax))
            {
                stageScript.positionPin++;
                MoveTo(stageScript.positionsPlayerPin[stageScript.positionPin].transform.position, true);

                SetJourneyPosition();
                LookAtRightPin();
            }
            
            AmIFirstorLastPos();
        }
        public void MoveToTheLeftDot()
        {            
            if (stageScript.positionPin > 1)
            {
                stageScript.positionPin--;
                MoveTo(stageScript.positionsPlayerPin[stageScript.positionPin].transform.position, true);

                SetJourneyPosition();
                LookAtLeftPin();
            }
            AmIFirstorLastPos();
        }

        public void ResetPosLetter()
        {
            if (AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100)//Letter is on a pin
            {
                MoveTo(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.position);
                stageScript.positionPin = stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>().pos;
                if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < stageScript.ropes.Length)
                    transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1].transform);
                else transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform);
            } else  //Letter is on a dot
              {
                MoveTo(stageScript.ropes[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1].transform.position);
                stageScript.positionPin = stageScript.ropes[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1].GetComponent<Dot>().pos;
                transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform);
            }
            //AmIFirstorLastPos();
        }
        public void ResetPosLetterAfterChangeStage()
        {
            stageScript.positionPin = 1;
            MoveTo(stageScript.positionsPlayerPin[1].transform.position);
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = 1;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 1;
            LookAtRightPin();
            UpdateCurrentJourneyPosition();
        }
        void SetJourneyPosition()
        {
            if (stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<Dot>() != null)
            {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<Dot>().playSessionActual;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<Dot>().learningBlockActual;

            }
            else
            {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
               stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapPin>().playSessionPin;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapPin>().learningBlockPin;

            }
            UpdateCurrentJourneyPosition();
        }
        void UpdateCurrentJourneyPosition()
        {
            AppManager.I.Player.SetCurrentJourneyPosition(new JourneyPosition(AppManager.I.Player.CurrentJourneyPosition.Stage,
             AppManager.I.Player.CurrentJourneyPosition.LearningBlock,
              AppManager.I.Player.CurrentJourneyPosition.PlaySession), true);
        }
        void LookAtRightPin()
        {
            LookAt(false);
        }
        void LookAtLeftPin()
        {
            LookAt(true);
        }

        void LookAt(bool leftPin)
        {
            rotateTween.Kill();
            Quaternion currRotation = this.transform.rotation;
            this.transform.LookAt(leftPin
                ? stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform.position
                : stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.position
            );
            Quaternion toRotation = this.transform.rotation;
            this.transform.rotation = currRotation;
            rotateTween = this.transform.DORotate(toRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);
        }
        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        void MoveTo(Vector3 position, bool animate = false)
        {
            if (moveTween != null) moveTween.Complete();
            if (animate) moveTween = this.transform.DOMove(position, 0.25f);
            else this.transform.position = position;
        }

        public void AmIFirstorLastPos()
        {
            if (stageScript.positionPin == 1)
            {
                if (stageScript.positionPinMax == 1)
                {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(false);
                }
                else
                {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(true);
                }

            }
            else if (stageScript.positionPin == stageScript.positionPinMax)
            {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(false);
            }
            else
            {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(true);
            }
        }
    }
}

