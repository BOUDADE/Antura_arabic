﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


namespace EA4S.SickLetters
{

    public enum letterStatus { idle, angry, horry}

    public class SickLettersLLPrefab : MonoBehaviour
    {

        public TextMeshPro dotlessLetter, correctDot;
        public SickLettersDraggableDD correctDotCollider;
        public SickLettersGame game;
        public LetterObjectView letterView;
        public letterStatus LLStatus = letterStatus.idle;
        public Animator letterAnimator;
        public List<SickLettersDraggableDD> thisLLWrongDDs = new List<SickLettersDraggableDD>();


        private SkinnedMeshRenderer[] LLMesh;
        Vector3 statPos;

        // Use this for initialization
        void Start()
        {
            LLMesh = GetComponentsInChildren<SkinnedMeshRenderer>();
            letterView = GetComponent<LetterObjectView>();
            letterAnimator = GetComponent<Animator>();
            statPos = transform.position;
            
        }

        // Update is called once per frame
        void Update()
        {
            //if(LLStatus == letterStatus.idle)
            //  letterAnimator.SetFloat("random", -1);
            //letterView.SetState(LLAnimationStates.LL_dancing);
            
        }

        public void jumpIn()
        {
            StartCoroutine(coJumpIn());
        }


        public void jumpOut(float delay = 0, bool endGame = false) {
            StartCoroutine(coJumpOut(delay, endGame));
        }

        IEnumerator coJumpIn()
        {
            showLLMesh(true);
            getNewLetterData();
            scatterDDs();

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<CapsuleCollider>().isTrigger = false;
            letterView.Falling = true;
            yield return new WaitForSeconds(0.30f);

            letterView.OnJumpEnded();
            letterAnimator.SetBool("dancing", game.LLCanDance);
            //letterView.SetState(LLAnimationStates.LL_idle);
            //letterAnimator.SetBool("idle", true);

            yield return new WaitForSeconds(1f);
            SickLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letterView.Data, true);

            if (game.roundsCount <1)
                game.tut.doTutorial(thisLLWrongDDs[Random.Range(0, thisLLWrongDDs.Count-1)].transform);
            
        }

        IEnumerator coJumpOut(float delay, bool endGame)
        {

            letterAnimator.SetBool("dancing", false);
            yield return new WaitForSeconds(delay );
            //GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_idle);
            letterAnimator.Play("LL_idle_1", -1);
            //letterView.SetState(LLAnimationStates.LL_still);
            game.manager.holeON();
            yield return new WaitForSeconds(0.25f);

            letterView.Falling = true;
            GetComponent<CapsuleCollider>().isTrigger = true;

            yield return new WaitForSeconds(.25f);
            game.Poof(transform.position + Vector3.up *8.5f - Vector3.forward);
            showLLMesh(false);
            yield return new WaitForSeconds(.75f);

            if (!endGame)
            {
                transform.position = new Vector3(statPos.x, 29.04f, statPos.z);
                StartCoroutine(coJumpIn());
            }
        }

        public void getNewLetterData()
        {
            //Temp Hack
            /*SickLettersQuestionProvider newQuestionProvider = SickLettersConfiguration.Instance.SickLettersQuestions;
            SickLettersQuestionsPack nextQuestionPack = newQuestionProvider.SickLettersGetNextQuestion();
            ILivingLetterData newLetter = (nextQuestionPack).GetQuestion();
            */

            /*IQuestionProvider newQuestionProvider = SickLettersConfiguration.Instance.Questions;
            IQuestionPack nextQuestionPack = newQuestionProvider.GetNextQuestion();
            ILivingLetterData newLetter = ((SickLettersQuestionsPack)nextQuestionPack).GetQuestion();*/

            ILivingLetterData newLetter = game.questionManager.getNewLetter();

            game.LLPrefab.GetComponent<LetterObjectView>().Init(newLetter);
            game.LLPrefab.dotlessLetter.text = newLetter.TextForLivingLetter;
            game.LLPrefab.correctDot.text = newLetter.TextForLivingLetter;

            
            //correctDotPos = LLPrefab.correctDot.transform.TransformPoint(Vector3.Lerp(LLPrefab.correctDot.mesh.vertices[0], game.LLPrefab.correctDot.mesh.vertices[2], 0.5f));

        }

        int i = 0;
        public void scatterDDs()
        {
            i = 0;
            //thisLLWrongDDs.Clear();

            foreach (SickLettersDropZone dz in game.DropZones)
            {
                if (dz.letters.Contains(game.LLPrefab.dotlessLetter.text))
                {
                    if (i < game.Draggables.Length)
                    {
                        if (game.Draggables[i].diacritic != Diacritic.None && !game.with7arakat)
                        {
                            i++;
                            continue;
                        }
                        SickLettersDraggableDD newDragable = game.createNewDragable(game.Draggables[i].gameObject);
                        newDragable.transform.parent = dz.transform;
                        newDragable.transform.localPosition = Vector3.zero;
                        newDragable.transform.localEulerAngles = new Vector3(0, -90, 0);
                        newDragable.setInitPos(newDragable.transform.localPosition);
                        //newDragable.isAttached = true;

                        thisLLWrongDDs.Add(newDragable);
                        game.allWrongDDs.Add(newDragable);

                        i++;
                    }
                }
            }
        }

        void showLLMesh(bool show)
        {
            foreach (SkinnedMeshRenderer sm in LLMesh)
                sm.enabled = show;
            correctDot.gameObject.SetActive(show);
            dotlessLetter.gameObject.SetActive(show);
        }
    }
}
